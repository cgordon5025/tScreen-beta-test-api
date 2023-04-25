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
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace GraphQl.GraphQl.Features.Objects.Adventure
{
    [ExtendObjectType("Query")]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AdventureQuery
    {
        
        public async Task<Models.Adventure?> GetAdventure(
            Guid id,
            AdventureByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
            => await dataLoader.LoadAsync(id, cancellationToken);

        [UseApplicationDbContext]
        public IQueryable<Models.Adventure> GetAdventures(
            [ScopedService] ApplicationDbContext context,
            [Service] IMapper mapper) 
            => context.CoreAdventures
                .TagWith(nameof(GetAdventures))
                .TagWithCallSiteSafely()
                .Where(e => e.DeletedAt == null)
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => mapper.Map<Models.Adventure>(e));
    }
}