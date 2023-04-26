using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Application.Mappings;
using AutoMapper;
using Core;
using Core.Extensions;
using Core.Settings;
using Core.Settings.Models;
using Core.TelemetryInitializers;
using Data;
using Domain.Entities.Identity;
using GraphQl.GraphQl.Features;
using GraphQl.Mappings;
using Infrastructure;
using Infrastructure.Configurations;
using MediatR;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GraphQl
{
    public class Startup
    {
        private readonly ApplicationEnvironment _environment;
        private readonly ILogger<Startup> _logger;
        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            _environment = new ApplicationEnvironment(hostEnvironment);

            _logger = NullLogger<Startup>.Instance;

            // Only in unprotected environments should the logger function
            if (_environment.IsKnownProtectedEnvironment())
                return;

            using var loggerFactory = LoggerFactory
                .Create(builder => builder
                    .SetMinimumLevel(LogLevel.Debug)
                    .AddConsole());

            _logger = loggerFactory.CreateLogger<Startup>();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var applicationName =
                _environment.GetEnvironmentVariableOrDefault(EnvironmentVariableNames.ApplicationName,
                    "Default.Local");

            services.UseValidateSettings();

            services.ConfigureValidateSettings<AzureAdSettings>(Configuration.GetSection("AzureAd"));

            services.ConfigureSettings<tScreenClientSettings>(
                Configuration.GetSection(tScreenClientSettings.SectionName));

            services.ConfigureSettings<SendGridSettings>(Configuration.GetSection("SendGrid"));

            services.ConfigureValidateSettings<AzureBlobStorageSettings>(Configuration.GetSection("AppStorage"),
                out var blobStorageSettings);

            services.ConfigureValidateSettings<KnownWebClientsSettings>(Configuration.GetSection("KnownWebClients"),
                out var knownWebClientsSettings);

            services.ConfigureValidateSettings<tScreenMssqlSettings>(Configuration.GetSection("tScreenMssql"),
                out var tScreenMssqlSettings);

            if (_environment.IsContainerHosted())
            {
                _logger.LogInformation("Application host: {Host}",
                    _environment.GetEnvironmentVariableOrDefault(EnvironmentVariableNames.ApplicationHost, "Local"));
            }

            IronPdf.License.LicenseKey = Configuration["IronPdf:LicenseKey"];
            if (_environment.IsDevelopment())
                _logger.LogInformation("IronPdf License Key: {Key}", Configuration["IronPdf:LicenseKey"]?.Obscure(27));

            if (!IronPdf.License.IsLicensed)
            {
                _logger.LogCritical("Iron PDF appears to not have a valid license");
            }

            if (_environment.ShouldUseLocalDevelopmentServices())
            {
                // Handy service/tool great for local development purposes
                // For production and other environments we use Azure Application Insights
                services.AddLogging(builder =>
                {
                    builder.AddSeq(Configuration.GetSection("Seq"));
                });
            }

            // If the APPLICATION_HOST is "Azure" then we force use of a token credential to authenticate 
            // with Azure storage account. If the APPLICATION_HOST is "Container" then the application is
            // hosted in a Docker container, either single, or with a composition of services
            // (e.g., docker compose). If an Azure Managed Instance ID or a Service Principal ID is 
            // provided then use this path. Else, default to a connection string (which requires an Account
            // Name and Key)
            if (_environment.IsAzureHosted() || _environment.IsContainerHosted() &&
                !string.IsNullOrWhiteSpace(blobStorageSettings?.UserAssignedId) ||
                (blobStorageSettings.TenantId is not null && blobStorageSettings.ClientId is not null
                                                      && blobStorageSettings.ClientSecret is not null))
            {
                _logger.LogDebug("Using persistent storage with token credential");

                var credential = blobStorageSettings.GetTokenCredential();
                services
                    .AddDataProtection()
                    .SetApplicationName(applicationName)
                    .PersistKeysToAzureBlobStorage(
                        blobStorageSettings.GetStorageEndpointUri("private/keys.xml"),
                        credential)
                    .UseCryptographicAlgorithms(
                        new AuthenticatedEncryptorConfiguration
                        {
                            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                            ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                        }); ;
            }
            else
            {
                // If the application is not in a docker container, and thus,
                // locally hosted (APPLICATION_HOST = "Local) we'll use localhost instead of the
                // azurite custom DNS name (which for most cases will be for a Docker Network).
                // We're expecting a local configuration to be a bridged network
                if (_environment.IsLocallyHosted())
                    blobStorageSettings.DnsName = null;

                _logger.LogDebug("Using persistent storage with connection string");
                services
                    .AddDataProtection()
                    .PersistKeysToAzureBlobStorage(blobStorageSettings.GetConnectionString(), "private", "keys.xml");
            }

            services.AddApplicationInsightsTelemetry();
            services.AddSingleton<ITelemetryInitializer, CloudRoleTelemetryInitializer>(_ =>
                new CloudRoleTelemetryInitializer(applicationName));

            services.AddSingleton<ITelemetryInitializer, ApplicationTelemetryInitializer>();

            _logger.LogDebug("MSSQL Connection String {ConnectionString}", tScreenMssqlSettings.ConnectionString);

            services.AddPooledDbContextFactory<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(_environment.IsContainerHosted()
                        ? @"Server=sqlserver;Database=tScreen;User Id=sa;Password=DevelopmentPassword123;"
                        : tScreenMssqlSettings.ConnectionString);
            });

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services
                .AddApiAuthentication(Configuration, _environment)
                .AddApiAuthorization();

            services.PostConfigure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState.ErrorsToHashtable();
                    return new BadRequestObjectResult(new
                    {
                        message = "Details provided in the model are missing or invalid",
                        status = StatusCodes.Status400BadRequest,
                        errors = errors
                    });
                };
            });

            // Make a DBContext pool instance available as a singleton (identity framework consideration)
            services.AddSingleton(service =>
                service.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

            // services.AddApplication();

            // AutoMapper and Mediatr 
            // Must be defined before injection of services
            services
                .AddAutoMapperAndMediatr()
                .AddInfrastructure(Configuration)
                .AddGraphQlServices();

            services
                .AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            // services.AddAzureClients(builder =>
            // {
            //     builder.AddClient<QueueClient, QueueClientOptions>((options, _, _) =>
            //     {
            //         options.MessageEncoding = QueueMessageEncoding.Base64;
            //         return new QueueClient(blobStorageSettings.ConnectionString, "tscreen-worklist-data", options);
            //     });
            // });

            services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(type => type.ToString());

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "tScreen.Api",
                    Description = "Private tScreen API",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = Configuration.GetSection("App:AdminName").Value,
                        Email = Configuration.GetSection("App:AdminEmail").Value
                    },
                    License = new OpenApiLicense
                    {
                        Name = "UNLICENSED",
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition(nameof(SecuritySchemeType.OAuth2), new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.OAuth2,
                    Name = nameof(HttpRequestHeader.Authorization),
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri("/api/oauth/authenticate", UriKind.Relative),
                            Scopes = null
                        }
                    }
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            In = ParameterLocation.Header,
                            Name = nameof(SecuritySchemeType.OAuth2),
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = nameof(SecuritySchemeType.OAuth2)
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                c.EnableAnnotations();
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .WithOrigins(knownWebClientsSettings.Domains.ToArray())
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });

                options.AddPolicy("AllowAllPolicy", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger(o => o.RouteTemplate = "/docs/{documentName}/openapi.json");
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/docs/v1/openapi.json", "tScreen.Api v1");
                    c.EnableDeepLinking();
                    c.RoutePrefix = "docs";
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            if (env.IsDevelopment())
            {
                app.UseCors("AllowAllPolicy");
            }
            else
            {
                app.UseCors();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGraphQL();
            });
        }
    }

    public static class ServicesExtensions
    {
        public static IServiceCollection AddAutoMapperAndMediatr(this IServiceCollection services)
        {
            var executingAssembly = typeof(Startup).Assembly;

            var autoMapperConfig = new MapperConfiguration(config =>
            {
                config.AddProfile<AppMapProfile>();
                config.AddProfile<CoreMapProfile>();
                config.AddProfile<AdminMapProfile>();
                config.AddProfile<GraphQlMapProfile>();
                config.AddProfile<ViewModelMapProfile>();
            });

            services.AddSingleton(autoMapperConfig.CreateMapper());
            services.AddAutoMapper(executingAssembly);

            var assemblies = new[]
            {

                AppDomain.CurrentDomain.Load("Application"),
                AppDomain.CurrentDomain.Load("Infrastructure"),
                executingAssembly
            };

            services.AddMediatR(assemblies);

            return services;
        }
    }
}