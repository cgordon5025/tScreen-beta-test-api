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
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace GraphQl.GraphQl.Features.Objects.QuestionContingent;

[ExtendObjectType("Query"), Authorize]
public class QuestionContingentQuery
{
    public async Task<Models.QuestionContingent> GetQuestionContingent(
        Guid id,
        QuestionContingentByIdDataLoader dataLoader,
        CancellationToken cancellationToken)
        => await dataLoader.LoadAsync(id, cancellationToken);
    
    [UseApplicationDbContext]
    public IQueryable<Models.QuestionContingent> GetQuestionContingents(
        [ScopedService] ApplicationDbContext context,
        [Service] IMapper mapper)
        => context.CoreQuestionContingents
            .TagWith(nameof(GetQuestionContingents))
            .TagWithCallSiteSafely()
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => mapper.Map<Models.QuestionContingent>(e));
}