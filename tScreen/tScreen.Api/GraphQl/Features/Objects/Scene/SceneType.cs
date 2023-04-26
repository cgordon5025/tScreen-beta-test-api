using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Core.Extensions;
using Data;
using GraphQl.GraphQl.DataLoaders;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace GraphQl.GraphQl.Features.Objects.Scene
{
    public class SceneType : ObjectType<Models.Scene>
    {
        protected override void Configure(IObjectTypeDescriptor<Models.Scene> descriptor)
        {
            descriptor.Field(e => e.SceneQuestions)
                .UseDbContext<ApplicationDbContext>()
                .ResolveWith<SceneResolvers>(r =>
                    r.GetQuestions(default!, default!, default!, default))
                .Name("questions");

            descriptor.Field(e => e.Adventure)
                .UseDbContext<ApplicationDbContext>()
                .ResolveWith<SceneResolvers>(r =>
                    r.GetAdventureAsync(default!, default!, default))
                .Name("adventure");
        }

        private class SceneResolvers
        {
            public async Task<IEnumerable<Models.Question>> GetQuestions(
                [Parent] Models.Scene scene,
                QuestionByIdDataLoader dataLoader,
                [ScopedService] ApplicationDbContext context,
                CancellationToken cancellationToken
                )
            {
                var questionIds = await context.CoreScenes
                    .TagWith(nameof(GetQuestions))
                    .TagWithCallSiteSafely()
                    .Include(e => e.SceneQuestions)
                    .Where(e => e.Id == scene.Id)
                    .SelectMany(e => e.SceneQuestions.Select(x => x.QuestionId))
                    .ToArrayAsync(cancellationToken);

                return await dataLoader.LoadAsync(questionIds, cancellationToken);
            }

            public async Task<Models.Adventure> GetAdventureAsync(
                [Parent] Models.Scene scene,
                AdventureByIdDataLoader dataLoader,
                CancellationToken cancellationToken
            ) => await dataLoader.LoadAsync(scene.AdventureId, cancellationToken);
        }
    }
}