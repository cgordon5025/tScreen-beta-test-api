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
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

// ReSharper disable ClassNeverInstantiated.Global

namespace GraphQl.GraphQl.Features.Objects.Company
{
    [ExtendObjectType("Query")]
    public class CompanyQuery
    {
        public async Task<Models.Company?> GetCompany(
            Guid id, 
            CompanyDataLoader dataLoader,
            CancellationToken cancellationToken)
            => await dataLoader.LoadAsync(id, cancellationToken);

        [UseApplicationDbContext]
        public IQueryable<Models.Company> GetCompanies(
            [ScopedService] ApplicationDbContext context,
            [Service] IMapper mapper)
            => context.Company
                .TagWith(nameof(GetCompanies))
                .TagWithCallSiteSafely()
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => mapper.Map<Models.Company>(e));
    }
}