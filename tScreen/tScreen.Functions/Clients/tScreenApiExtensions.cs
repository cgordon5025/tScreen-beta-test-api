using Core.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace tScreen.Functions.Clients;

public static class tScreenApiExtensions
{
    public static IServiceCollection AddtScreenApiHttpClient(this IServiceCollection services, IConfiguration config)
    {
        services.ConfigureValidateSettings<tScreenApiSettings>(config.GetSection(tScreenApiSettings.SectionName));
        services.AddHttpClient<ItScreenApiHttpClient, tScreenApiHttpClient>();
        return services;
    }
}