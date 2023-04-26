using Pulumi;

namespace Shared.Resources;

public interface IComponentResourceTag
{
    public InputMap<string>? Tags { get; init; }
}