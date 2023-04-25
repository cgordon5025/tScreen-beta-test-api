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

// ReSharper disable ClassNeverInstantiated.Local

namespace GraphQl.GraphQl.Features.Objects.WorkList;

public class WorkListType : ObjectType<Models.WorkList>
{
    protected override void Configure(IObjectTypeDescriptor<Models.WorkList> descriptor)
    {
        descriptor.Field(e => e.LocationId)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<WorkListResolvers>(r
                => r.GetLocationAsync(default!, default!, default))
            .Name("location");
        
        descriptor.Field(e => e.PersonId)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<WorkListResolvers>(r
                => r.GetPersonAsync(default!, default!, default))
            .Name("person");
        
        descriptor.Field(e => e.SessionId)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<WorkListResolvers>(r
                => r.GetSessionAsync(default!, default!, default))
            .Name("session");

        descriptor.Field(e => e.WorkListNotes)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<WorkListResolvers>(r => 
                r.GetNotesAsync(default!, default!, default!, default))
            .Name("notes");
    }

    private class WorkListResolvers
    {
        public async Task<Models.Location?> GetLocationAsync(
            [Parent] Models.WorkList workList,
            LocationByIdDataLoader dataLoader,
            CancellationToken cancellationToken) 
            => await dataLoader.LoadAsync(workList.LocationId, cancellationToken);
        
        public async Task<Models.Person?> GetPersonAsync(
            [Parent] Models.WorkList workList,
            PersonByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
            => await dataLoader.LoadAsync(workList.PersonId, cancellationToken);
        
        public async Task<Models.Session?> GetSessionAsync(
            [Parent] Models.WorkList workList,
            SessionDataByIdDataLoader dataLoader,
            CancellationToken cancellationToken) 
            => await dataLoader.LoadAsync(workList.SessionId, cancellationToken);

        public async Task<IEnumerable<Models.Note>> GetNotesAsync(
            [Parent] Models.WorkList workList,
            [ScopedService] ApplicationDbContext context,
            NoteByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
        {
            var noteIds = await context.WorkList
                .TagWith(nameof(GetNotesAsync))
                .TagWithCallSiteSafely()
                .Include(e => e.WorkListNotes)
                .Where(e => e.Id == workList.Id)
                .SelectMany(e => e.WorkListNotes.Select(x => x.NoteId))
                .ToListAsync(cancellationToken);

            return await dataLoader.LoadAsync(noteIds, cancellationToken);
        }
    }
}