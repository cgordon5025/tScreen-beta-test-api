using System;
using GraphQl.GraphQl.Interfaces;

namespace GraphQl.GraphQl.Features.Objects.WorkList.Results;

public class WorkListsAlreadyAddedError : IWorkListResult, IErrorResult
{
    public string? Message { get; init; } = null!;
    public DateTime? ChangedAt { get; init; }
}