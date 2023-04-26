using Pulumi.AzureNative.Compute;
using Pulumi.AzureNative.Network;
using System.Net.NetworkInformation;

namespace tScreen.Infra.Core.Shared;

public class VmMachine
{
    public VirtualMachine VirtualMachine { get; init; } = null!;
    public NetworkInterface NetworkInterface { get; init; } = null!;
}