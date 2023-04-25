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

namespace GraphQl.GraphQl.Features.Objects.Question
{
    public class QuestionType : ObjectType<Models.Question>
    {
        protected override void Configure(IObjectTypeDescriptor<Models.Question> descriptor)
        {
            descriptor
                .Field(e => e.SceneQuestions)
                .UseDbContext<ApplicationDbContext>()
                .ResolveWith<QuestionResolvers>(r => 
                    r.GetScenes(default!, default!, default!, default))
                .Name("scenes");

            descriptor.Field(e => e.SceneQuestions)
                .UseDbContext<ApplicationDbContext>()
                .ResolveWith<QuestionResolvers>(r =>
                    r.GetAdventuresAsync(default!, default!, default!, default))
                .Name("adventures");

            descriptor.Field(e => e.QuestionContingents)
                .UseDbContext<ApplicationDbContext>()
                .ResolveWith<QuestionResolvers>(r =>
                    r.GetQuestionContingents(default!, default!, default!, default))
                .Name("questionContingents");
        }
        
        private class QuestionResolvers
        {
            public async Task<IEnumerable<Models.Scene>> GetScenes(
                [Parent] Models.Question question,
                SceneByIdDataLoader dataLoader,
                [ScopedService] ApplicationDbContext context,
                CancellationToken cancellationToken)
            {
                var sceneIds = await context.CoreQuestions
                    .TagWith(nameof(GetScenes))
                    .TagWithCallSiteSafely()
                    .Include(e => e.SceneQuestions)
                    .Where(e => e.Id == question.Id)
                    .SelectMany(e => e.SceneQuestions.Select(x => x.Id))
                    .ToArrayAsync(cancellationToken);

                return await dataLoader.LoadAsync(sceneIds, cancellationToken);
            }

            public async Task<IEnumerable<Models.Adventure>> GetAdventuresAsync(
                [Parent] Models.Question question,
                AdventureByIdDataLoader dataLoader,
                [ScopedService] ApplicationDbContext context,
                CancellationToken cancellationToken)
            {
                var adventureIds = await context.CoreQuestions
                    .TagWith(nameof(GetAdventuresAsync))
                    .TagWithCallSiteSafely()
                    .Include(e => e.SceneQuestions)
                    .ThenInclude(e => e.Scene)
                    .Where(e => e.Id == question.Id)
                    .SelectMany(e => e.SceneQuestions.Select(x => x.Scene!.AdventureId))
                    .ToArrayAsync(cancellationToken);

                return await dataLoader.LoadAsync(adventureIds, cancellationToken);
            }

            public async Task<IEnumerable<Models.QuestionContingent>> GetQuestionContingents(
                [Parent] Models.Question question,
                QuestionContingentByIdDataLoader dataLoader,
                [ScopedService] ApplicationDbContext context,
                CancellationToken cancellationToken)
            {
                var questionContingentIds = await context.CoreQuestions
                    .TagWith(nameof(GetQuestionContingents))
                    .TagWithCallSiteSafely()
                    .Include(e => e.QuestionParentContingents)
                    .Where(e => e.Id == question.Id)
                    .SelectMany(e => e.QuestionParentContingents.Select(x => x.Id))
                    .ToArrayAsync(cancellationToken);

                return await dataLoader.LoadAsync(questionContingentIds, cancellationToken);
            }
        }
    }
}