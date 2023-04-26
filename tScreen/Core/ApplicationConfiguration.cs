using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Core
{
    public class ApplicationConfiguration : IApplicationConfiguration
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public string ApplicationRootDirectory { get; }

        private readonly IApplicationEnvironment _environment;

        public ApplicationConfiguration(IApplicationEnvironment environment)
        {
            ApplicationRootDirectory = Directory.GetCurrentDirectory();

            _environment = environment;
        }

        public IConfiguration GetConfiguration(string userSecretsId)
            => GetConfigurationBuilder(userSecretsId)
                .Build();

        public IConfiguration GetConfiguration(Assembly assembly)
            => GetConfigurationBuilder(assembly)
                .Build();

        public IConfigurationBuilder GetConfigurationBuilder(string userSecretsId)
        {
            var configurationBuilder = _getConfigurationBuilder();

            if (_environment.IsKnownUnprotectedEnvironment())
            {
                configurationBuilder.AddUserSecrets(userSecretsId);
            }

            return configurationBuilder;
        }

        public IConfigurationBuilder GetConfigurationBuilder(Assembly assembly)
        {
            var configurationBuilder = _getConfigurationBuilder();

            if (_environment.IsKnownUnprotectedEnvironment())
            {
                configurationBuilder.AddUserSecrets(assembly);
            }

            return configurationBuilder;
        }

        private IConfigurationBuilder _getConfigurationBuilder()
        {
            return new ConfigurationBuilder()
                .SetBasePath(ApplicationRootDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{_environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }
    }
}