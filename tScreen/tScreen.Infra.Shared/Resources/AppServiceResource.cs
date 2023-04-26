using Pulumi;
using Pulumi.AzureNative.Resources;

namespace Shared.Resources;

public class AppServiceResource
{
    public AppServiceResource()
    {

    }
}

public class AppServiceResourceArgs
{
    public string? AppServicePlanId { get; set; }
    public string? ManagedIdentityId { get; set; }
    public ResourceGroup? ResourceGroup { get; set; }
    public InputMap<string>? Tags { get; set; }
}