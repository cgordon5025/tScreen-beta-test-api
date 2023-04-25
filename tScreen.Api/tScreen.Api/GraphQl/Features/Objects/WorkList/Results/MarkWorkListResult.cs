using GraphQl.GraphQl.Interfaces;

namespace GraphQl.GraphQl.Features.Objects.WorkList.Results;

public class MarkWorkListResult : IWorkListResult
{
    public string Message { get; init; } = null!;
    public string Code { get; set; } = null!;
}