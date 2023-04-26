using HotChocolate;

namespace GraphQl.GraphQl.Attributes;

public class PersonIdAttribute : GlobalStateAttribute
{
    public PersonIdAttribute() : base("PersonId") { }
}