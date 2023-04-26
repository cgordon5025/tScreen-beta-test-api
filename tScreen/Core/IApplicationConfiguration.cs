using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Core
{
    public interface IApplicationConfiguration
    {
        IConfiguration GetConfiguration(string userSecretsId);
        IConfiguration GetConfiguration(Assembly assembly);
        IConfigurationBuilder GetConfigurationBuilder(string userSecretsId);
        IConfigurationBuilder GetConfigurationBuilder(Assembly assembly);
    }
}