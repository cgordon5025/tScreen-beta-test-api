using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
namespace Core.Settings;

public static class ValidateSettingsStartupFilterExtension
{
    public static IServiceCollection UseValidateSettings(this IServiceCollection services)
        => services.AddTransient<IStartupFilter, ValidateSettingsStartupFilter>();

    public static IServiceCollection ConfigureSettings<TOptions>(this IServiceCollection services,
        IConfiguration configuration) where TOptions : class, IValidateSettings, new()
    {
        services.Configure<TOptions>(configuration);

        services.AddSingleton(context => context.GetRequiredService<IOptions<TOptions>>().Value);
        return services;
    }

    public static IServiceCollection ConfigureValidateSettings<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration
    ) where TOptions : class, IValidateSettings, new()
    {
        services.Configure<TOptions>(configuration);

        services.AddSingleton(context => context.GetRequiredService<IOptions<TOptions>>().Value);

        services.AddSingleton<IValidateSettings>(context => context
            .GetRequiredService<IOptions<TOptions>>().Value);

        return services;
    }

    public static IServiceCollection ConfigureValidateSettings<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration,
        out TOptions settings
    ) where TOptions : class, IValidateSettings, new()
    {
        settings = configuration.Get<TOptions>();

        services.Configure<TOptions>(configuration);

        services.AddSingleton(context => context.GetRequiredService<IOptions<TOptions>>().Value);

        services.AddSingleton<IValidateSettings>(context => context
            .GetRequiredService<IOptions<TOptions>>().Value);

        return services;
    }

    public static IConfiguration TryValidateSettings<TOptions>(
        this IConfiguration configuration,
        string sectionName,
        out TOptions settings) where TOptions : class, IValidateSettings, new()
    {
        settings = configuration.GetSection(sectionName).Get<TOptions>();
        SettingsValidator.Validate(settings);
        return configuration;
    }
}