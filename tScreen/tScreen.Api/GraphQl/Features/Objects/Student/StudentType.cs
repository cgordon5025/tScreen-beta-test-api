using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Extensions;
using Data;
using Domain.Entities.App;
using GraphQl.GraphQl.DataLoaders;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace GraphQl.GraphQl.Features.Objects.Student;

public class StudentType : ObjectType<Models.Student>
{
    protected override void Configure(IObjectTypeDescriptor<Models.Student> descriptor)
    {
        descriptor.Field(e => e.Dob)
            .ResolveWith<StudentResolvers>(r => r.GetDateTime(default!));

        descriptor.Field(e => e.Locations)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<StudentResolvers>(r =>
                r.GetLocationAsync(default!, default!, default))
            .Name("location");

        descriptor.Field(e => e.Avatars)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<StudentResolvers>(r =>
                r.GetAvatarsAsync(default!, default!, default!, default))
            .Name(("avatars"));

        descriptor.Field(e => e.StudentCustomFields)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<StudentResolvers>(r =>
                r.GetCustomFieldsAsync(default!, default!, default!, default));

        descriptor.Field(e => e.Sessions)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<StudentResolvers>(r =>
                r.GetSessionsAsync(default!, default!, default!, default!));

        descriptor.Field(e => e.LastSession)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<StudentResolvers>(r =>
                r.GetLastSessionAsync(default!, default!, default!, default!));
    }

    private class StudentResolvers
    {
        public async Task<Models.Location> GetLocationAsync(
            [Parent] Models.Student student,
            LocationByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
            => await dataLoader.LoadAsync(student.LocationId, cancellationToken);

        public async Task<IEnumerable<Models.Avatar>> GetAvatarsAsync(
            [Parent] Models.Student student,
            [ScopedService] ApplicationDbContext context,
            AvatarByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
        {
            var avatarIds = await context.AppAvatars.Where(e => e.StudentId == student.Id)
                .TagWith(nameof(GetAvatarsAsync))
                .TagWithCallSiteSafely()
                .OrderBy(e => e.CreatedAt)
                .Select(e => e.Id)
                .ToListAsync(cancellationToken);

            var avatarsDictionary = await dataLoader
                .LoadAsync(avatarIds, cancellationToken);

            // We need to order the results to ensure the latest avatar 
            var avatars = avatarsDictionary.OrderByDescending(e => e.CreatedAt);

            return avatars;
        }

        public async Task<IEnumerable<Models.StudentCustomField>> GetCustomFieldsAsync(
            [Parent] Models.Student student,
            [ScopedService] ApplicationDbContext context,
            [Service] IMapper mapper,
            CancellationToken cancellationToken
        )
        {
            var entities = await context.StudentCustomField
                .TagWith(nameof(GetCustomFieldsAsync))
                .TagWithCallSiteSafely()
                .Include(e => e.CustomField)
                .Where(e => e.StudentId == student.Id)
                .ToListAsync(cancellationToken);

            return mapper.Map<IEnumerable<Models.StudentCustomField>>(entities);
        }

        public async Task<IEnumerable<Models.Session>> GetSessionsAsync(
            [Parent] Models.Student student,
            [ScopedService] ApplicationDbContext context,
            [Service] IMapper mapper,
            CancellationToken cancellationToken)
        {
            var entities = await context.AppSessions
                .TagWith($"{nameof(StudentType)}.{nameof(GetSessionsAsync)}")
                .TagWithCallSiteSafely()
                .OrderByDescending(e => e.CreatedAt)
                .Where(e => e.StudentId == student.Id)
                .ToListAsync(cancellationToken);

            return mapper.Map<IEnumerable<Models.Session>>(entities);
        }

        public DateTime GetDateTime([Parent] Models.Student student)
        {
            return student.Dob.Date;
        }

        public async Task<Models.Session?> GetLastSessionAsync(
            [Parent] Models.Student student,
            [ScopedService] ApplicationDbContext context,
            [Service] IMapper mapper,
            CancellationToken cancellationToken)
        {
            var entity = await context.AppSessions
                .TagWith($"{nameof(StudentType)}.{nameof(GetLastSessionAsync)}")
                .TagWithCallSiteSafely()
                .Where(e => e.StudentId == student.Id && e.Checkpoint == SessionCheckpoints.AdventureComplete)
                .OrderByDescending(e => e.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            return mapper.Map<Models.Session>(entity);
        }
    }
}