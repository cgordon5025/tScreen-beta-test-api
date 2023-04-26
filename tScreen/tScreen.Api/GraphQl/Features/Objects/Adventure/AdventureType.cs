
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

namespace GraphQl.GraphQl.Features.Objects.Adventure
{
    public class AdventureType : ObjectType<Models.Adventure>
    {
        protected override void Configure(IObjectTypeDescriptor<Models.Adventure> descriptor)
        {
            descriptor.Field(e => e.CoreFile)
                .UseDbContext<ApplicationDbContext>()
                .ResolveWith<AdventureResolvers>(r =>
                    r.GetFile(default!, default!, default))
                .Name("file");

            descriptor.Field(e => e.Scenes)
                .UseDbContext<ApplicationDbContext>()
                .ResolveWith<AdventureResolvers>(r =>
                    r.GetScenesAsync(default!, default!, default!, default))
                .Name("scenes");
        }

        private class AdventureResolvers
        {
            public async Task<Models.CoreFile?> GetFile(
                [Parent] Models.Adventure adventure,
                CoreFileByIdDataLoader dataLoader,
                CancellationToken cancellationToken)
                => await dataLoader.LoadAsync(adventure.FileId, cancellationToken);

            public async Task<IEnumerable<Models.Scene>> GetScenesAsync(
                [Parent] Models.Adventure adventure,
                SceneByIdDataLoader dataLoader,
                [ScopedService] ApplicationDbContext context,
                CancellationToken cancellationToken)
            {
                var sceneIds = await context.CoreScenes
                    .TagWith(nameof(GetScenesAsync))
                    .TagWithCallSiteSafely()
                    .Where(e => e.AdventureId == adventure.Id)
                    .Select(e => e.Id)
                    .ToListAsync(cancellationToken);

                return await dataLoader.LoadAsync(sceneIds, cancellationToken);
            }
        }
    }
}