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

namespace GraphQl.GraphQl.Features.Objects.CustomField
{
    [ExtendObjectType("Query")]
    public class CustomFieldQuery
    {
        public async Task<Models.CustomField?> GetCustomField(
            Guid id,
            CustomFieldByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
            => await dataLoader.LoadAsync(id, cancellationToken);

        [UseApplicationDbContext]
        public IQueryable<Models.CustomField> GetCustomFields(
            [ScopedService] ApplicationDbContext context,
            [Service] IMapper mapper) 
            => context.CustomField
                .TagWith(nameof(GetCustomFields))
                .TagWithCallSiteSafely()
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => mapper.Map<Models.CustomField>(e));
    }
}