using Pulumi;
using TwingateLabs.Twingate.Inputs;

namespace Shared.Resources;

public class TwingateNetworkResource
{
    public string Name { get; init; } = null!;

    public Input<string> Label { get; init; } = null!;
    public Input<string> Address { get; init; } = null!;
    public InputList<string> GroupIds { get; init; } = null!;
    public TwingateResourceProtocolsArgs? Protocols { get; init; }
}