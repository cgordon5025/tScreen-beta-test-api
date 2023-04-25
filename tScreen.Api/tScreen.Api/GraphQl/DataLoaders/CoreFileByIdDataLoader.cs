using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Extensions;
using Data;
using GraphQl.GraphQl.Models;
using GreenDonut;
using Microsoft.EntityFrameworkCore;

namespace GraphQl.GraphQl.DataLoaders
{
    public class CoreFileByIdDataLoader : BatchDataLoader<Guid, CoreFile>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public CoreFileByIdDataLoader(
            IDbContextFactory<ApplicationDbContext> contextFactory,
            IMapper mapper,
            IBatchScheduler batchScheduler, 
            DataLoaderOptions? options = null) 
            : base(batchScheduler, options)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        protected override async Task<IReadOnlyDictionary<Guid, CoreFile>> LoadBatchAsync(IReadOnlyList<Guid> keys, CancellationToken cancellationToken)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            return await context.CoreFiles
                .TagWith($"{nameof(CoreFileByIdDataLoader)}.{nameof(LoadBatchAsync)}")
                .TagWithCallSiteSafely()
                .Where(e => keys.Contains(e.Id))
                .Select(e => _mapper.Map<CoreFile>(e))
                .ToDictionaryAsync(e => e.Id, cancellationToken);
        }
    }
}