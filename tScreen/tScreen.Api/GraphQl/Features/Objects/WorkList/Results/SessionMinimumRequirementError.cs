using GraphQl.GraphQl.Interfaces;

namespace GraphQl.GraphQl.Features.Objects.WorkList.Results;

public class SessionMinimumRequirementError : IWorkListResult, IErrorResult
{
    public string? Message { get; init; } = null!;
    public string CurrentStatus { get; init; } = null!;
    public string ExpectedStatus { get; init; } = null!;
}