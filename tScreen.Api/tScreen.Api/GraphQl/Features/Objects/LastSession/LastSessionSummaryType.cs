using System.Collections.Generic;
using System.Threading.Tasks;
using HotChocolate.Types;

namespace GraphQl.GraphQl.Features.Objects.LastSession;

public class LastSessionType : ObjectType<Models.Session>
{
    // public class Summary
    // {
    //     public string SpendTimeWith { get; set; } = null!;
    //     public int NumberOfSiblings { get; set; }
    //     public ICollection<string> DependsOn { get; set; } = new List<string>();
    // }
    //
    // protected override void Configure(IObjectTypeDescriptor<Summary> descriptor)
    // {
    //     descriptor.Field(e => e.SpendTimeWith).Name("Answers");
    // }
    //
    // // private class LastSessionSummaryResolvers
    // // {
    // //     public Task Get
    // // }
}