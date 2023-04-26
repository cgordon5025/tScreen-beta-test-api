﻿using GraphQl.GraphQl.Interfaces;

namespace GraphQl.GraphQl.Features.Objects.WorkList.Results;

public class WorkListNotFoundError : IWorkListResult, IErrorResult
{
    public string? Message { get; init; } = null!;
    public string Id { get; init; } = null!;
}