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

// ReSharper disable ClassNeverInstantiated.Global

namespace GraphQl.GraphQl.Features.Objects.Location
{
    [ExtendObjectType("Query")]
    public class LocationQuery
    {
        public async Task<Models.Location?> GetLocation(
            Guid id,
            LocationByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
            => await dataLoader.LoadAsync(id, cancellationToken);
        
        [UseApplicationDbContext]
        public IQueryable<Models.Location> GetLocations(
            [ScopedService] ApplicationDbContext context,
            [Service] IMapper mapper)
        {
            return context.Location
                .TagWith(nameof(GetLocations))
                .TagWithCallSiteSafely()
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => mapper.Map<Models.Location>(e));
        }
    }
}