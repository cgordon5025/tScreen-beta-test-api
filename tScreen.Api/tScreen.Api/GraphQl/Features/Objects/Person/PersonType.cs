using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Extensions;
using Data;
using GraphQl.GraphQl.DataLoaders;
using GraphQl.GraphQl.Features.Objects.WorkList;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ClassNeverInstantiated.Local

namespace GraphQl.GraphQl.Features.Objects.Person;

/// <inheritdoc />
public class PersonType : ObjectTypeExtension<Models.Person>
{
    protected override void Configure(IObjectTypeDescriptor<Models.Person> descriptor)
    {
        descriptor.Field(e => e.CompanyId)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<PersonResolvers>(r => r.GetCompanyAsync(default!, default!, default))
            .Name("company");

        descriptor.Field(e => e.LocationPersons)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<PersonResolvers>(r => 
                r.GetLocationsAsync(default!, default!, default!, default))
            .Name("locations");

        descriptor.Field(e => e.PersonStudents)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<PersonResolvers>(r =>
                r.GetStudentsAsync(default!, default!, default!, default))
            .Name("students");

        descriptor.Field(e => e.WorkLists)
            .UseDbContext<ApplicationDbContext>()
            .Argument("status", d => d.Type<EnumType<WorkListStatus?>>())
            .ResolveWith<PersonResolvers>(r =>
                r.GetWorkListsAsync(default, default!, default!, default!, default))
            .Name("workLists");
        
        descriptor.Field(e => e.HistoryPersons)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<PersonResolvers>(r =>
                r.GetHistoryAsync(default!, default!, default!, default))
            .Name("history");

        descriptor.Field(e => e.Sessions)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<PersonResolvers>(r =>
                r.GetSessionsAsync(default!, default!, default!, default))
            .Name("sessions");
    }

    private class PersonResolvers
    {
        public async Task<Models.Company> GetCompanyAsync(
            [Parent] Models.Person person,
            CompanyDataLoader dataLoader,
            CancellationToken cancellationToken
        ) => await dataLoader.LoadAsync(person.CompanyId, cancellationToken);

        public async Task<IEnumerable<Models.Location>> GetLocationsAsync(
            [Parent] Models.Person person,
            [ScopedService] ApplicationDbContext context,
            LocationByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
        {
            var locationIds = await context.Person
                .TagWith(nameof(GetLocationsAsync))
                .TagWithCallSiteSafely()
                .Include(e => e.LocationPersons)
                .Where(e => e.Id == person.Id)
                .SelectMany(e => e.LocationPersons.Select(x => x.LocationId))
                .ToListAsync(cancellationToken);

            return await dataLoader.LoadAsync(locationIds, cancellationToken);
        }

        public async Task<IEnumerable<Models.Student>> GetStudentsAsync(
            [Parent] Models.Person person,
            [ScopedService] ApplicationDbContext context,
            StudentByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
        {
            var studentIds = await context.Person
                .TagWith(nameof(GetStudentsAsync))
                .TagWithCallSiteSafely()
                .Include(e => e.PersonStudents)
                .Where(e => e.Id == person.Id)
                .SelectMany(e => e.PersonStudents.Select(x => x.StudentId))
                .ToListAsync(cancellationToken);

            return await dataLoader.LoadAsync(studentIds, cancellationToken);
        }
        
        public async Task<IEnumerable<Models.WorkList>> GetWorkListsAsync(
            WorkListStatus? status,
            [Parent] Models.Person person,
            [ScopedService] ApplicationDbContext context,
            WorkListByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
        {
            var query = context.Person
                .TagWith(nameof(GetWorkListsAsync))
                .TagWithCallSiteSafely()
                .Include(e => e.WorkLists)
                .Where(e => e.Id == person.Id);
            
            var queryIds = status is null
                ? query.SelectMany(e => e.WorkLists.Select(x => x.Id))
                : query.SelectMany(e => e.WorkLists
                    .Where(x => x.Status == status.Value).Select(x => x.Id));

            var workListIds = await queryIds.ToListAsync(cancellationToken);
            
            return await dataLoader.LoadAsync(workListIds, cancellationToken);
        }

        public async Task<IEnumerable<Models.History>> GetHistoryAsync(
            [Parent] Models.Person person,
            [ScopedService] ApplicationDbContext context,
            HistoryByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
        {
            var workListIds = await context.Person
                .Include(e => e.HistoryPersons)
                .Where(e => e.Id == person.Id)
                .SelectMany(e => e.HistoryPersons.Select(x => x.HistoryId))
                .ToListAsync(cancellationToken);

            return await dataLoader.LoadAsync(workListIds, cancellationToken);
        }

        public async Task<IEnumerable<Models.Session>> GetSessionsAsync(
            [Parent] Models.Person person,
            [ScopedService] ApplicationDbContext context,
            SessionDataByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
        {
            var sessionIds = await context.AppSessions
                .TagWith($"{nameof(PersonType)}.{nameof(GetSessionsAsync)}")
                .TagWithCallSiteSafely()
                .Where(e => e.PersonId == person.Id)
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => e.Id)
                .ToListAsync(cancellationToken);

            return await dataLoader.LoadAsync(sessionIds, cancellationToken);
        }
    }
}