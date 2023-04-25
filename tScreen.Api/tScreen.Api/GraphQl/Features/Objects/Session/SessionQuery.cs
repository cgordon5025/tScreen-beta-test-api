using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Extensions;
using Data;
using GraphQl.GraphQl.Attributes;
using GraphQl.GraphQl.DataLoaders;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

// ReSharper disable ClassNeverInstantiated.Global

namespace GraphQl.GraphQl.Features.Objects.Session;

[ExtendObjectType("Query")]
public class SessionQuery
{
    public async Task<Models.Session> GetSession(
        Guid id,
        SessionDataByIdDataLoader dataLoader,
        CancellationToken cancellationToken)
        => await dataLoader.LoadAsync(id, cancellationToken);

    [UseApplicationDbContext]
    public IQueryable<Models.Session> GetSessions(
        Guid locationId,
        SessionStatus? status,
        [ScopedService] ApplicationDbContext context,
        [Service] IMapper mapper) 
    {
        
        var query = context.AppSessions
            .TagWith(nameof(GetSessions))
            .TagWithCallSiteSafely()
            .Where(e => e.LocationId == locationId);

        if (status is not null)
            query = query.Where(e => e.Status == status.Value);

        return query.OrderByDescending(e => e.CreatedAt)
            .Select(e => mapper.Map<Models.Session>(e));
    }

    public TestType GetSessionStatusType(SessionStatus status) => new TestType
    {
        Id = status.Id,
        Name = status.Name,
        Value = status.Value
    };
}

public class TestType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
}