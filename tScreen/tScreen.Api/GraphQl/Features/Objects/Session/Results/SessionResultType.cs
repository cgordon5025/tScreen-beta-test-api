using System;
using System.Collections.Generic;
using GraphQl.GraphQl.Interfaces;
using GraphQl.GraphQl.Models;
using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace GraphQl.GraphQl.Features.Objects.Session.Results;

public class DuplicateCheckpointError : ISessionResult, IErrorResult
{
    public string? Message { get; init; }
    public string Checkpoint { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
}

public class SessionNotFoundError : ISessionResult, IErrorResult
{
    public string? Message { get; init; }
    public string? Id { get; init; }
}

public class SessionAlreadyClosedError : ISessionResult, IErrorResult
{
    public string? Message { get; init; }
    public DateTime? ClosedAt { get; init; }
}

public static class SessionResultExtension
{
    public static IRequestExecutorBuilder AddSessionResultTypes(this IRequestExecutorBuilder builder)
    {
        builder
            .AddType<SessionNotFoundError>()
            .AddType<DuplicateCheckpointError>()
            .AddType<SessionAlreadyClosedError>();

        return builder;
    }
}