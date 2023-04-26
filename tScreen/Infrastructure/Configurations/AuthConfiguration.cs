using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Core;
using Core.Settings;
using Core.Settings.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Identity.Web;

namespace Infrastructure.Configurations;

public static class AuthConfiguration
{
    public static IServiceCollection AddApiAuthentication(this IServiceCollection services,
        IConfiguration configuration, ApplicationEnvironment environment)
    {
        services.ConfigureValidateSettings<JwtSettings>(configuration.GetSection("Jwt"), out var settings);

        var issuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.SigningKey));

        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<IIdentityService, IdentityService>();

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidIssuer = settings.Authority,
                    ValidAudience = settings.Audience,
                    IssuerSigningKey = issuerSigningKey
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        if (string.IsNullOrWhiteSpace(context.Error))
                            context.Error = "invalid_token";

                        if (string.IsNullOrWhiteSpace(context.ErrorDescription))
                            context.ErrorDescription = "Valid JWT is required";

                        if (context.AuthenticateFailure != null
                            && context.AuthenticateFailure.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            var exception = context.AuthenticateFailure as SecurityTokenExpiredException;
                            var expiredFormatted = exception?.Expires.ToString("o");
                            context.Response.Headers.Add("x-token-expired", expiredFormatted);
                            context.ErrorDescription = $"The token expired on {expiredFormatted}";
                        }

                        return context.Response.WriteAsJsonAsync(new
                        {
                            error = context.Error,
                            error_description = context.ErrorDescription
                        });
                    },

                    OnMessageReceived = context =>
                    {
                        // https://github.com/whatwg/html/issues/3062
                        var accessToken = context.Request.Headers["access_token"];
                        if (!string.IsNullOrWhiteSpace(accessToken))
                            context.Token = accessToken;

                        return Task.CompletedTask;
                    }
                };
            })
            // .AddJwtBearer("SessionBearer", options =>
            // {
            //     options.Authority = settings.Authority;
            //     options.Audience = settings.StudentAudience;
            //     options.TokenValidationParameters = new TokenValidationParameters
            //     {
            //         // ValidateIssuer = false,
            //         // ValidateAudience = false
            //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.SigningKey+"abcd"))
            //     };
            //     options.Events = new JwtBearerEvents
            //     {
            //         OnTokenValidated = context =>
            //         {
            //             var identity = (ClaimsIdentity)context.Principal?.Identity!;
            //             identity.AddClaim(new Claim("schema", "SessionBearer"));
            //             return Task.CompletedTask;
            //         },
            //         OnMessageReceived = context =>
            //         {
            //             var identity = (ClaimsIdentity)context.Principal?.Identity!;
            //             identity.AddClaim(new Claim("schema", "SessionBearer"));
            //             return Task.CompletedTask;
            //         }
            //     };
            // })
            .AddMicrosoftIdentityWebApi(configuration, configSectionName: "AzureAd",
                jwtBearerScheme: "AzureAd");

        if (environment.IsDevelopment())
        {
            // Decode class name to actual objects. Important: this feature should be
            // available only in the development environment due to sensitive nature
            // of the data revealed.
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        }

        return services;
    }

    public static IServiceCollection AddApiAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme, "AzureAd")
                .Build();

            options.AddPolicy("AllowAnonymous", policyBuilder =>
            {
                // Remove authentication requirement when using this policy
                // Because GraphQL doesn't allow as of yet the use of the AllowAnonymous functions
                policyBuilder.Requirements.Add(new AllowAnonymousHandler());
            });

            options.AddPolicy("AzureAd", policyBuilder =>
            {
                policyBuilder
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes("AzureAd");
            });
        });

        services.AddSingleton<IAuthorizationHandler, AllowAnonymousHandler>();

        return services;
    }

    public static IServiceCollection AddAuthorizations(this IServiceCollection services)
    {
        return services;
    }
}