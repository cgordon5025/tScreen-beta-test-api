using System;
using System.Collections.Generic;
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
using HotChocolate.Data.Projections.Expressions;
using HotChocolate.Data.Sorting.Expressions;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

// ReSharper disable ClassNeverInstantiated.Global

namespace GraphQl.GraphQl.Features.Objects.Scene
{
    [ExtendObjectType("Query")]
    public class SceneQuery
    {
        public async Task<Models.Scene> GetScene(
            Guid id,
            SceneByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
            => await dataLoader.LoadAsync(id, cancellationToken);
        
        [UseApplicationDbContext]
        public IEnumerable<Models.Scene> GetScenes(
            Guid? adventureId,
            [ScopedService] ApplicationDbContext context,
            IResolverContext resolverContext)
        {
            var query = context.CoreScenes
                .TagWith(nameof(GetScenes))
                .TagWithCallSiteSafely()
                .AsQueryable();

            if (adventureId is not null)
                query = query.Where(e => e.AdventureId == adventureId);

            return query.Select(e => new Models.Scene
            {
                Id = e.Id,
                Status = e.Status,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                ArchivedAt = e.ArchivedAt,
                DeletedAt = e.DeletedAt,
                AdventureId = e.AdventureId,
                Name = e.Name,
                Description = e.Description,
                Position = e.Position
            }).AsEnumerable();
        }
        
        
    }
}