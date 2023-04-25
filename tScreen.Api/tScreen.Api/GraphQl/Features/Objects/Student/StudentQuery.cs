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

namespace GraphQl.GraphQl.Features.Objects.Student
{
    [ExtendObjectType("Query")]
    public class StudentQuery
    {
        public async Task<Models.Student> GetStudent(
            Guid id,
            StudentByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
            => await dataLoader.LoadAsync(id, cancellationToken);

        [UseApplicationDbContext]
        [UsePaging(IncludeTotalCount = true, MaxPageSize = 100)]
        public IQueryable<Models.Student> GetStudents(
            string? query,
            [CurrentPersonContext] CurrentPersonContext currentPersonContext,
            [ScopedService] ApplicationDbContext context,
            [Service] IMapper mapper)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return context.PersonStudent.Include(e => e.Student)
                    .TagWith(nameof(GetStudents))
                    .TagWithCallSiteSafely()
                    .Where(e => e.PersonId == currentPersonContext.PersonId)
                    .OrderByDescending(e => e.Student!.CreatedAt)
                    .Select(e => mapper.Map<Models.Student>(e.Student));
            }

            return context.PersonStudent
                .TagWith($"{nameof(GetStudents)}--Search")
                .TagWithCallSiteSafely()
                .Include(e => e.Student)
                .Where(e => e.PersonId == currentPersonContext.PersonId
                            && (e.Student.FirstName.Contains(query) || e.Student.LastName.StartsWith(query) ||
                                e.Student.StudentCustomFields.Any(e => e.Value.Contains(query))))
                .OrderByDescending(e => e.Student!.CreatedAt)
                .Select(e => mapper.Map<Models.Student>(e.Student));
        }
    }
}