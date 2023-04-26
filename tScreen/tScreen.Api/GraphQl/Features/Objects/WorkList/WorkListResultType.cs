using GraphQl.GraphQl.Features.Objects.WorkList.Results;
using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQl.GraphQl.Features.Objects.WorkList;

public static class WorkListResultType
{
    public static IRequestExecutorBuilder AddWorkListResultTypes(this IRequestExecutorBuilder builder)
    {
        builder
            .AddType<WorkListsCreatedResult>()
            .AddType<MarkWorkListResult>()
            .AddType<WorkListsAlreadyAddedError>()
            .AddType<WorkListNotFoundError>();

        return builder;
    }
}