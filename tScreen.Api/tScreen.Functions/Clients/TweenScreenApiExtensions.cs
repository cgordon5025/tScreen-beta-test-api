using Core.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TweenScreen.Functions.Clients;

public static class TweenScreenApiExtensions
{
    public static IServiceCollection AddTweenScreenApiHttpClient(this IServiceCollection services, IConfiguration config)
    {
        services.ConfigureValidateSettings<TweenScreenApiSettings>(config.GetSection(TweenScreenApiSettings.SectionName));
        services.AddHttpClient<ITweenScreenApiHttpClient, TweenScreenApiHttpClient>();
        return services;
    }
}