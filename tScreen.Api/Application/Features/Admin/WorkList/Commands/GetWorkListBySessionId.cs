using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Admin.Models;
using AutoMapper;
using Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.WorkList.Commands;

public class GetWorkListsBySessionId : IRequest<IEnumerable<WorkListDTO>>
{
    public Guid SessionId { get; init; }

    sealed class GetWorkListsBySessionIdHandler : IRequestHandler<GetWorkListsBySessionId, IEnumerable<WorkListDTO>>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public GetWorkListsBySessionIdHandler(IDbContextFactory<ApplicationDbContext> contextFactory, 
            IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }
        
        public async Task<IEnumerable<WorkListDTO>> Handle(GetWorkListsBySessionId request, 
            CancellationToken cancellationToken)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await context.WorkList
                .Include(e => e.Person)
                .Where(e => e.SessionId == request.SessionId)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IEnumerable<WorkListDTO>>(entity);
        }
    }
}