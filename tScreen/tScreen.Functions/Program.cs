using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Core;
using Core.Settings;
using Core.Settings.Models;
using Core.TelemetryInitializers;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using tScreen.Functions.Clients;

namespace tScreen.Functions
{
    public class Program
    {
        public static async Task Main()
        {
            var environment = new ApplicationEnvironment();
            var applicationConfigurationBuilder = new ApplicationConfiguration(environment)
                .GetConfigurationBuilder(typeof(Program).Assembly);

            var hostBuilder = new HostBuilder();

            // Utilize operating system functions
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                hostBuilder.UseWindowsService();
            // Systemd is not available in docker environments because it's generally considered bad
            // practice to load and control multiple processes in a single Docker container. It's 
            // possible to enable systemd with parameter "--cap-add SYS_ADMIN" passed to Docker CLI
            // but it's considered bad security practice to do so. Simply systemd should not be used
            // for process control in a docker containerized environment. Supervisord is a nice 
            // alternative, but services should be isolated by container, unless good reason is
            // given otherwise
            else if (!environment.IsContainerHosted() && (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                     || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)))
                hostBuilder.UseSystemd();

            hostBuilder.ConfigureAppConfiguration(configBuilder =>
            {
                var configuration = applicationConfigurationBuilder.Build();
                configBuilder.Sources.Clear();
                configBuilder.AddConfiguration(configuration);

                // Loads configurations which is injected with the environment is a local Docker 
                // compose running instance.
                if (environment.IsContainerHosted())
                    configBuilder.AddJsonFile("appsettings.Docker.json", true);

                if (!environment.IsAzureHosted()) return;

                configuration.TryValidateSettings<KeyVaultSetting>("KeyVault", out var keyVaultSettings);

                TokenCredential credential;

                if (string.IsNullOrWhiteSpace(keyVaultSettings.UserAssignedId))
                {
                    credential = new ClientSecretCredential(
                        keyVaultSettings.TenantId,
                        keyVaultSettings.ClientId,
                        keyVaultSettings.ClientSecret);
                }
                else
                {
                    credential = new ChainedTokenCredential(
                        new ManagedIdentityCredential(keyVaultSettings.UserAssignedId),
                        new AzureCliCredential());
                }

                configBuilder.AddAzureKeyVault(new Uri(keyVaultSettings.VaultUri), credential,
                    new AzureKeyVaultConfigurationOptions
                    {
                        Manager = new KeyVaultSecretManager(),
                        ReloadInterval = TimeSpan.FromHours(24)
                    });
            })
            .ConfigureWebJobs(builder =>
            {
                builder.AddAzureStorageCoreServices();
                builder.AddAzureStorageBlobs();
                builder.AddAzureStorageQueues();
                builder.AddTimers();
            })
            .ConfigureLogging((context, builder) =>
            {
                builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                builder.ClearProviders();
                builder.AddConsole();

                if (environment.ShouldUseLocalDevelopmentServices())
                    builder.AddSeq(context.Configuration.GetSection("Seq"));

                builder.AddApplicationInsightsWebJobs(options =>
                {
                    var instrumentationKey = context.Configuration["ApplicationInsights:InstrumentationKey"];
                    if (!string.IsNullOrWhiteSpace(instrumentationKey))
                        options.InstrumentationKey = instrumentationKey;
                });
            })
            .ConfigureServices((context, services) =>
            {
                services.ConfigureValidateSettings<AzureAdClientSettings>(
                    context.Configuration.GetSection("AzureAdClient"));

                var applicationName = Environment
                    .GetEnvironmentVariable(EnvironmentVariableNames.ApplicationName);

                services.AddSingleton<ITokenProvider, TokenProvider>();
                services.AddSingleton<ITelemetryInitializer, CloudRoleTelemetryInitializer>(_ =>
                    new CloudRoleTelemetryInitializer(
                        environment.IsContainerHosted()
                            ? applicationName ?? typeof(Program).Assembly.FullName
                            : $"{applicationName}.Worker"));

                services.AddtScreenApiHttpClient(context.Configuration);
            })
            .UseEnvironment(environment.GetEnvironmentVariableOrDefault(environment.EnvironmentName, "Development"))
            .UseConsoleLifetime();

            using var cancellationToken = new CancellationTokenSource();
            using var host = hostBuilder.Build();
            await host.RunAsync(cancellationToken.Token);
        }
    }
}