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
using GraphQl.GraphQl.Features.Objects.Question.Inputs;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

// ReSharper disable ClassNeverInstantiated.Global

namespace GraphQl.GraphQl.Features.Objects.Question
{
    [ExtendObjectType("Query"), Authorize]
    public class QuestionQuery
    {
        public async Task<Models.Question?> GetQuestion(
            Guid id,
            QuestionByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
            => await dataLoader.LoadAsync(id, cancellationToken);

        [UseApplicationDbContext]
        public IQueryable<Models.Question> GetQuestions(
            QuestionCategoryType? category,
            [ScopedService] ApplicationDbContext context,
            [Service] IMapper mapper)
        {
            var query = context.CoreQuestions
                .TagWith(nameof(GetQuestions))
                .TagWithCallSiteSafely()
                .AsQueryable();

            if (category is not null)
                query = query.Where(e => e.Category == category.ToString());

            return query.OrderBy(e => e.Category)
                .ThenBy(e => e.Position)
                .Select(e => mapper.Map<Models.Question>(e));
        }
    }
}