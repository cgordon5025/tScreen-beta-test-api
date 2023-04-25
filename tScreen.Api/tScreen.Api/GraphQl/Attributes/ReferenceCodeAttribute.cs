using HotChocolate;

namespace GraphQl.GraphQl.Attributes;

public class ReferenceCodeAttribute : GlobalStateAttribute
{
    public const string Name = "ReferenceCode";

    public ReferenceCodeAttribute() : base(Name) { }
}