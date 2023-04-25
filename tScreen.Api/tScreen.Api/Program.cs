using System;
using Azure.Core;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Core;
using Core.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace GraphQl
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var environment = new ApplicationEnvironment(context.HostingEnvironment);
                    var configuration = builder.Build();

                    // Loads configurations which is injected with the environment is a local Docker 
                    // compose running instance. 
                    if (environment.IsContainerHosted())
                        builder.AddJsonFile("appsettings.Docker.json", true);

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
                        
                    builder.AddAzureKeyVault(new Uri(keyVaultSettings.VaultUri), credential,
                        new AzureKeyVaultConfigurationOptions
                        {
                            Manager = new KeyVaultSecretManager(),
                            ReloadInterval = TimeSpan.FromHours(24)
                        });
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    if (OperatingSystem.IsLinux())
                    {
                        webBuilder.UseKestrel(options =>
                        {
                            var timeout = TimeSpan.FromMinutes(30);
                            options.Limits.KeepAliveTimeout = timeout;
                            options.Limits.RequestHeadersTimeout = timeout;
                        });
                    }

                    webBuilder.UseStartup<Startup>();
                });
    }
}