using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Extensions;
using Data;
using GraphQl.GraphQl.Attributes;
using GraphQl.GraphQl.DataLoaders;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace GraphQl.GraphQl.Features.Objects.WorkList;

[ExtendObjectType("Query")]
public class WorkListQuery
{
    public async Task<Models.WorkList> GetWorkList(
        Guid id,
        WorkListByIdDataLoader dataLoader,
        CancellationToken cancellationToken)
        => await dataLoader.LoadAsync(id, cancellationToken);

    [UseApplicationDbContext]
    public IQueryable<Models.WorkList> GetWorkLists(
        [ScopedService] ApplicationDbContext context,
        [Service] IMapper mapper)
        => context.WorkList
            .TagWith(nameof(GetWorkLists))
            .TagWithCallSiteSafely()
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => mapper.Map<Models.WorkList>(e));
}