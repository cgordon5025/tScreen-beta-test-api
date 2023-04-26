using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GraphQl.GraphQl.Attributes;
using GraphQl.GraphQl.DataLoaders;
using HotChocolate.Types;

// ReSharper disable ClassNeverInstantiated.Global

namespace GraphQl.GraphQl.Features.Objects.Me;

[ExtendObjectType("Query")]
public class MeQuery
{
    public async Task<Models.Person?> GetMe(
        [CurrentPersonContext] CurrentPersonContext currentPersonContext,
        PersonByIdDataLoader dataLoader,
        CancellationToken cancellationToken)
    {
        return await dataLoader.LoadAsync(currentPersonContext.PersonId, cancellationToken);
    }
}