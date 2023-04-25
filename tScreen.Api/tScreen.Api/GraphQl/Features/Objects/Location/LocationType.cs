using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Extensions;
using Data;
using GraphQl.GraphQl.DataLoaders;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace GraphQl.GraphQl.Features.Objects.Location
{
    public class LocationType : ObjectType<Models.Location>
    {
        protected override void Configure(IObjectTypeDescriptor<Models.Location> descriptor)
        {
            descriptor.Field(e => e.LocationPersons)
                .UseDbContext<ApplicationDbContext>()
                .ResolveWith<LocationResolvers>(r => r
                    .GetPersonsAsync(default!, default!, default!, default))
                .Name("people");

            descriptor.Field(e => e.Company)
                .UseDbContext<ApplicationDbContext>()
                .ResolveWith<LocationResolvers>(r => 
                    r.GetCompanyAsync(default!, default!, default))
                .Name("company");

            descriptor.Field(e => e.CustomFields)
                .UseDbContext<ApplicationDbContext>()
                .ResolveWith<LocationResolvers>(r =>
                    r.GetCustomFieldsAsync(default!, default!, default!, default));
        }

        private class LocationResolvers
        {
            public async Task<IEnumerable<Models.Person>> GetPersonsAsync(
                [Parent] Models.Location location,
                PersonByIdDataLoader dataLoader,
                [ScopedService] ApplicationDbContext context,
                CancellationToken cancellationToken
            )
            {
                var personIds = await context.Location
                    .TagWith(nameof(GetPersonsAsync))
                    .TagWithCallSiteSafely()
                    .Include(e => e.LocationPersons)
                    .Where(e => e.Id == location.Id)
                    .SelectMany(e => e.LocationPersons.Select(x => x.PersonId))
                    .ToListAsync(cancellationToken);

                return await dataLoader.LoadAsync(personIds, cancellationToken);
            }

            public async Task<Models.Company> GetCompanyAsync(
                [Parent] Models.Location location,
                CompanyDataLoader dataLoader,
                CancellationToken cancellationToken
            ) => await dataLoader.LoadAsync(location.CompanyId, cancellationToken);

            public async Task<IEnumerable<Models.CustomField>> GetCustomFieldsAsync(
                [Parent] Models.Location location, 
                CustomFieldByIdDataLoader dataLoader, 
                [ScopedService] ApplicationDbContext context, 
                CancellationToken cancellationToken)
            {
                var customFieldIds = await context.CustomField
                    .TagWith(nameof(GetCustomFieldsAsync))
                    .TagWithCallSiteSafely()
                    .Where(e => e.LocationId == location.Id)
                    .Select(e => e.Id)
                    .ToListAsync(cancellationToken);

                return await dataLoader.LoadAsync(customFieldIds, cancellationToken);
            }
        }
    }
}