using HotChocolate;

namespace GraphQl.GraphQl.Attributes;

public class CompanyIdAttribute : GlobalStateAttribute
{
    public const string Name = "CompanyId";
    
    public CompanyIdAttribute() : base(Name) { }   
}