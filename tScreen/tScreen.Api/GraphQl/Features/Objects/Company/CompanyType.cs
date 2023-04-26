

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Core.Extensions;
using Data;
using GraphQl.GraphQl.DataLoaders;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace GraphQl.GraphQl.Features.Objects.Company
{
    public class CompanyType : ObjectType<Models.Company>
    {
        protected override void Configure(IObjectTypeDescriptor<Models.Company> descriptor)
        {
            descriptor.Field(e => e.Locations)
                .UseDbContext<ApplicationDbContext>()
                .ResolveWith<CompanyResolvers>(r =>
                    r.GetLocations(default!, default!, default!, default!));

        }

        private class CompanyResolvers
        {
            public async Task<IEnumerable<Models.Location>> GetLocations(
                [Parent] Models.Company company,
                LocationByIdDataLoader dataLoader,
                [ScopedService] ApplicationDbContext context,
                CancellationToken cancellationToken
            )
            {
                var locationIds = await context.Company
                    .TagWith(nameof(GetLocations))
                    .TagWithCallSiteSafely()
                    .Include(e => e.Locations)
                    .Where(e => e.Id == company.Id)
                    .SelectMany(e => e.Locations.Select(x => x.Id))
                    .ToArrayAsync(cancellationToken);

                return await dataLoader.LoadAsync(locationIds, cancellationToken);
            }
        }
    }
}