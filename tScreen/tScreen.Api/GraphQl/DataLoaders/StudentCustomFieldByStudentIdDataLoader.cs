using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Extensions;
using Data;
using GraphQl.GraphQl.Models;
using GreenDonut;
using Microsoft.EntityFrameworkCore;

namespace GraphQl.GraphQl.DataLoaders;

public class StudentCustomFieldByStudentIdDataLoader : GroupedDataLoader<Guid, Models.StudentCustomField>
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly IMapper _mapper;

    public StudentCustomFieldByStudentIdDataLoader(
        IDbContextFactory<ApplicationDbContext> contextFactory,
        IMapper mapper,
        IBatchScheduler batchScheduler, DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _contextFactory = contextFactory;
        _mapper = mapper;
    }

    protected override async Task<ILookup<Guid, StudentCustomField>> LoadGroupedBatchAsync(
        IReadOnlyList<Guid> keys, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return context.StudentCustomField
            .TagWith($"{nameof(StudentCustomFieldByStudentIdDataLoader)}.{nameof(LoadGroupedBatchAsync)}")
            .TagWithCallSiteSafely()
            .Where(e => keys.Contains(e.StudentId))
            .Select(e => _mapper.Map<StudentCustomField>(e))
            .ToLookup(e => e.StudentId);
    }
}