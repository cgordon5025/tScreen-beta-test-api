using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Core.TelemetryInitializers;

public class ApplicationTelemetryInitializer : ITelemetryInitializer
{
    public void Initialize(ITelemetry telemetry)
    {
        var version = Environment.GetEnvironmentVariable(EnvironmentVariableNames.ApplicationVersion);
        if (version is not null && telemetry.Context.GlobalProperties.ContainsKey("AppVersion"))
            telemetry.Context.GlobalProperties.Add("AppVersion", version);

        var hash = Environment.GetEnvironmentVariable(EnvironmentVariableNames.ApplicationHash);
        if (hash is not null && telemetry.Context.GlobalProperties.ContainsKey("AppHash"))
            telemetry.Context.GlobalProperties.Add("AppHash", hash);

        var host = Environment.GetEnvironmentVariable(EnvironmentVariableNames.ApplicationHost);
        if (host is not null && telemetry.Context.GlobalProperties.ContainsKey("AppHost"))
            telemetry.Context.GlobalProperties.Add("AppHost", host);
    }
}