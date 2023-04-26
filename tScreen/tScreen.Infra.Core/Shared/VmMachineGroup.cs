using Pulumi;

namespace tScreen.Infra.Core.Shared;

public class VmMachineGroup
{
    public Output<string> Ip { get; set; } = null!;
    public Output<string> Public { get; init; } = null!;
    public Output<string> Private { get; init; } = null!;
}