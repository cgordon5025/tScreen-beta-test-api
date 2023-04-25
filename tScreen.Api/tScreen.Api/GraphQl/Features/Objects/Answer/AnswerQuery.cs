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

namespace GraphQl.GraphQl.Features.Objects.Answer;

[ExtendObjectType("Query")]
public class AnswerQuery
{
    public async Task<Models.Answer> GetAnswer(
        Guid id,
        AnswerByIdDataLoader dataLoader,
        CancellationToken cancellationToken)
        => await dataLoader.LoadAsync(id, cancellationToken);

    [UseApplicationDbContext]
    public IQueryable<Models.Answer> GetAnswers(
        [ScopedService] ApplicationDbContext context,
        [Service] IMapper mapper)
        => context.AppAnswers
            .TagWith(nameof(GetAnswers))
            .TagWithCallSiteSafely()
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => mapper.Map<Models.Answer>(e));
}