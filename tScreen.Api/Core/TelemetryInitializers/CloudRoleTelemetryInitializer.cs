using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Core.TelemetryInitializers;

public class CloudRoleTelemetryInitializer : ITelemetryInitializer
{
    private readonly string _roleName;

    public CloudRoleTelemetryInitializer(string roleName)
    {
        _roleName = roleName;
    }
    
    public void Initialize(ITelemetry telemetry)
    {
        telemetry.Context.Cloud.RoleName = _roleName;
    }
}