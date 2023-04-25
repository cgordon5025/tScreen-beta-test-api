using System;
using System.Linq;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Core
{
    public class ApplicationEnvironment : IApplicationEnvironment, IHostEnvironment
    {
        public ApplicationEnvironment()
        {
            EnvironmentName = Capitalize(GetEnvironmentVariableOrDefault(
                KnownEnvironments.EnvironmentVarName, KnownEnvironments.Production));
        }
        
        public ApplicationEnvironment(IHostEnvironment environment)
        {
            EnvironmentName = Capitalize(environment.EnvironmentName);
            ApplicationHost = Capitalize(GetEnvironmentVariableOrDefault(
                ApplicationHostNames.EnvironmentVarName,
                ApplicationHostNames.Default));
            
            ApplicationName = environment.ApplicationName;
            ContentRootPath = environment.ContentRootPath;
            ContentRootFileProvider = environment.ContentRootFileProvider;
        }
        
        public string EnvironmentName { get => _environmentName; set => _environmentName = Capitalize(value); }
        private string _environmentName; 
        
        public string ApplicationName { get; set; }
        public string ContentRootPath { get; set; }

        /// <summary>
        /// Types:
        ///     Local
        ///     Azure
        ///     Container
        ///     Azure Container
        /// </summary>
        public string ApplicationHost { get; set; } = "Local";
        
        public IFileProvider ContentRootFileProvider { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public static string GetEnvironmentVariableOrDefault(string key, string defaultValue)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (string.IsNullOrEmpty(defaultValue))
                throw new ArgumentNullException(nameof(defaultValue));

            try 
            {
                return !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(key))
                    ? Environment.GetEnvironmentVariable(key)
                    : defaultValue;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
        
        public bool IsEnvironment(string name) 
            => Capitalize(name) == EnvironmentName;

        public bool IsDevelopment() 
            => IsEnvironment(KnownEnvironments.Development);

        public bool IsTesting()
            => IsEnvironment(KnownEnvironments.Testing);

        public bool IsStaging()
            => IsEnvironment(KnownEnvironments.Staging);
        
        public bool IsProduction()
            => IsEnvironment(KnownEnvironments.Production);

        public bool IsAzureHosted() => ApplicationHost == ApplicationHostNames.Azure;
        public bool IsLocallyHosted() => ApplicationHost == ApplicationHostNames.Local;
        public bool IsContainerHosted() => ApplicationHost == ApplicationHostNames.Container;

        public bool ShouldUseLocalDevelopmentServices() =>
            GetEnvironmentVariableOrDefault(EnvironmentVariableNames.ApplicationUseLocalDevelopmentServices, false);
        
        public bool IsKnownUnprotectedEnvironment() 
            => KnownEnvironments.UnProtectedEnvironments.Contains(Capitalize(EnvironmentName));

        public bool IsKnownProtectedEnvironment()
            => KnownEnvironments.ProtectedEnvironments.Contains(Capitalize(EnvironmentName));

        public T GetEnvironmentVariableOrDefault<T>(string key, T defaultValue)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (defaultValue is null)
                throw new ArgumentNullException(nameof(defaultValue));

            try 
            {
                return !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(key))
                    ? (T) Convert.ChangeType(Environment.GetEnvironmentVariable(key), typeof(T))
                    : defaultValue;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        private static string Capitalize(string value)
            => value.First().ToString().ToUpper() + value[1..].ToLower();
    }

    public static class ApplicationHostNames
    {
        public const string EnvironmentVarName = "APPLICATION_HOST";
        public const string Local = nameof(Local);
        public const string Azure = nameof(Azure);
        public const string Container = nameof(Container);
        public const string Default = nameof(Local);
    }
    
    public static class KnownEnvironments
    {
        public const string EnvironmentVarName = "ASPNETCORE_ENVIRONMENT";
        public const string Development = nameof(Development);
        public const string Testing = nameof(Testing);
        public const string Staging = nameof(Staging);
        public const string Production = nameof(Production);
        public static readonly string[] UnProtectedEnvironments = {
            Development,
            Testing
        };
        public static readonly string[] ProtectedEnvironments = {
            Staging,
            Production
        };
    }
}