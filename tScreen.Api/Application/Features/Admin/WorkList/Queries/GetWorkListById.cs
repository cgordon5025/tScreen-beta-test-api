using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Admin.Models;
using AutoMapper;
using Core.Extensions;
using Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.WorkList.Queries;

public class GetWorkListById : IRequest<WorkListDTO>
{
    public Guid Id { get; set; }
    
    internal sealed class WorkListByIdHandler : IRequestHandler<GetWorkListById, WorkListDTO>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public WorkListByIdHandler(IDbContextFactory<ApplicationDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }
        
        public async Task<WorkListDTO> Handle(GetWorkListById request, CancellationToken cancellationToken)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await context.WorkList
                .TagWith(nameof(GetWorkListById))
                .TagWithCallSiteSafely()
                .Where(e => e.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            return _mapper.Map<WorkListDTO>(entity);
        }
    }
}