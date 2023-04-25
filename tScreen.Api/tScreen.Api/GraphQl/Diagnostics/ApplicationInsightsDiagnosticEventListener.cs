using System;
using System.Collections.Generic;
using GraphQl.GraphQl.Attributes;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Execution.Instrumentation;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace GraphQl.GraphQl.Diagnostics;

// ReSharper disable once ClassNeverInstantiated.Global
public class ApplicationInsightsDiagnosticEventListener : ExecutionDiagnosticEventListener
{
    private const string GraphQlQueryLabel = "GraphQlQuery";
    private const string GraphQlQueryHashLabel = "GraphQlQueryHash";
    
    private readonly TelemetryClient _telemetryClient;

    public ApplicationInsightsDiagnosticEventListener(TelemetryClient telemetryClient)
    {
        _telemetryClient = telemetryClient;
    }

    public override IDisposable ExecuteRequest(IRequestContext context)
    {
        var httpContext = GetHttpContext(context);
        if (httpContext is null)
            return EmptyScope;

        var queryHash = context.Request.QueryHash ?? context.Request.QueryId ?? "UnknownQueryId";
        var operationName = context.Operation?.Name ?? context.Request.OperationName ?? "UnknownOperation";
        var operationPath = $"{operationName}.{Uri.EscapeDataString(queryHash)}";
        var fullyQualifiedOperationPath = $"/graphql/{operationPath}";
        
        var requestTelemetry = new RequestTelemetry
        {
            Name = fullyQualifiedOperationPath,
            Url = new Uri($"{httpContext.Request.GetUri().AbsoluteUri}{operationPath}")
        };

        var operationId = GetOperationIdFrom(httpContext);
        requestTelemetry.Context.Operation.Id = operationId;
        requestTelemetry.Context.Operation.ParentId = operationId;
        requestTelemetry.Context.User.AuthenticatedUserId = httpContext.User.Identity?.Name ?? "Anonymous";
        requestTelemetry.Context.Operation.Name = $"GQL POST {fullyQualifiedOperationPath}";

        if (context.Request.Query is not null)
            requestTelemetry.Properties.Add(GraphQlQueryLabel, context.Request.Query.ToString());
        
        requestTelemetry.Properties.Add(nameof(context.Request.OperationName), operationName);
        requestTelemetry.Properties.Add(GraphQlQueryHashLabel, queryHash);
        
        if (context.ContextData.ContainsKey(ReferenceCodeAttribute.Name))
            requestTelemetry.Properties.Add(ReferenceCodeAttribute.Name, 
                context.ContextData[ReferenceCodeAttribute.Name]?.ToString());
        
        var operation = _telemetryClient.StartOperation(requestTelemetry);
        
        return new RequestScope(() => ExecutedRequestCompleted(context, operation));
    }

    private void ExecutedRequestCompleted(IRequestContext context, IOperationHolder<RequestTelemetry> operation)
    {
        var httpContext = GetHttpContext(context);

        operation.Telemetry.Success = httpContext?.Response.StatusCode is >= StatusCodes.Status200OK and <= 299;
        operation.Telemetry.ResponseCode = httpContext?.Response.StatusCode.ToString();

        if (context.Exception is not null)
        {
            operation.Telemetry.Success = false;
            operation.Telemetry.ResponseCode = StatusCodes.Status500InternalServerError.ToString();
            _telemetryClient.TrackException(context.Exception);
        }

        if (context.ValidationResult?.HasErrors ?? false)
        {
            operation.Telemetry.Success = false;
            operation.Telemetry.ResponseCode = StatusCodes.Status400BadRequest.ToString();
        }

        if (context.Result?.Errors is not null)
        {
            operation.Telemetry.Success = false;
            operation.Telemetry.ResponseCode = StatusCodes.Status400BadRequest.ToString();

            foreach (var error in context.Result.Errors)
                if (error.Exception is not null)
                    _telemetryClient.TrackException(error.Exception);
        }
        
        _telemetryClient.StopOperation(operation);
    }

    public override void RequestError(IRequestContext context, Exception exception)
    {
        _telemetryClient.TrackException(exception);
        base.RequestError(context, exception);
    }

    public override void ValidationErrors(IRequestContext context, IReadOnlyList<IError> errors)
    {
        foreach (var error in errors)
            _telemetryClient.TrackTrace($"GraphQL validation error: {error.Message}", SeverityLevel.Warning);
        
        base.ValidationErrors(context, errors);
    }

    private static HttpContext? GetHttpContext(IHasContextData context)
    {
        if (!context.ContextData.ContainsKey(nameof(HttpContext)))
            return null;

        return context.ContextData[nameof(HttpContext)] as HttpContext;
    }

    private static string GetOperationIdFrom(HttpContext httpContext) => httpContext.TraceIdentifier;
}

internal class RequestScope : IDisposable
{
    private readonly Action _disposeAction;

    public RequestScope(Action disposeAction)
    {
        _disposeAction = disposeAction;
    }
        
    public void Dispose()
    {
        _disposeAction.Invoke();
    }
}