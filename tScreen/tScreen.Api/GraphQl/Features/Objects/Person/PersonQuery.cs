using System;
using System.Linq;
using System.Reflection;
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

namespace GraphQl.GraphQl.Features.Objects.Person;

[ExtendObjectType("Query")]
public class PersonQuery
{
    public Task<Models.Person> GetPerson(
        Guid id,
        PersonByIdDataLoader dataLoader,
        CancellationToken cancellationToken)
        => dataLoader.LoadAsync(id, cancellationToken);

    [UseApplicationDbContext]
    public IQueryable<Models.Person> GetPersons(
        [ScopedService] ApplicationDbContext context,
        [Service] IMapper mapper)
    {
        return context.Person
            .TagWith(nameof(GetPersons))
            .TagWithCallSiteSafely()
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => mapper.Map<Models.Person>(e));
    }
}