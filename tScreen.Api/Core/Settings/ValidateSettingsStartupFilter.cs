using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Core.Settings;

public class ValidateSettingsStartupFilter : IStartupFilter
{
    private readonly IEnumerable<IValidateSettings> _settingsCollection;
    public ValidateSettingsStartupFilter(IEnumerable<IValidateSettings> settingsCollection)
    {
        _settingsCollection = settingsCollection;
    }
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {   
        foreach (var settings in _settingsCollection)
        {
            SettingsValidator.Validate(settings);
        }

        return next;
    }
}