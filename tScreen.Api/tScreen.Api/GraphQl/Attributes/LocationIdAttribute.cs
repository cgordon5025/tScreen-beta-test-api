using HotChocolate;

namespace GraphQl.GraphQl.Attributes;

public class LocationIdAttribute : GlobalStateAttribute
{
    public const string Name = "LocationId";
    public LocationIdAttribute() : base(Name) { }
}