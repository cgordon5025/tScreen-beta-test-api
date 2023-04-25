using GraphQl.GraphQl.Interfaces;

namespace GraphQl.GraphQl.Features.Objects.WorkList.Results;

public class WorkListsCreatedResult : IWorkListResult
{
    public string Message { get; init; } = null!;
    public string Code { get; set; } = null!;
}