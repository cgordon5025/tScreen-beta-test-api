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

namespace GraphQl.GraphQl.Features.Objects.Avatar;

[ExtendObjectType("Query")]
public class AvatarQuery
{
    public Task<Models.Avatar> GetAvatar(
        Guid id,
        AvatarByIdDataLoader dataLoader,
        CancellationToken cancellationToken)
        => dataLoader.LoadAsync(id, cancellationToken);

    [UseApplicationDbContext]
    [UseSorting]
    public IQueryable<Models.Avatar> GetAvatars(
        [ScopedService] ApplicationDbContext context,
        [Service] IMapper mapper)
        => context.AppAvatars
            .TagWith(nameof(GetAvatars))
            .TagWithCallSiteSafely()
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => mapper.Map<Models.Avatar>(e));
}