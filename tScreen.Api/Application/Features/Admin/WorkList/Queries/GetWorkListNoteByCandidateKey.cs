using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Features.Admin.Models;
using AutoMapper;
using Core.Extensions;
using Data;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.WorkList.Queries;

public class GetWorkListByCandidateKey : IRequest<WorkListNoteDTO>
{
    public Guid WorkListId { get; init; }
    public Guid NoteId { get; init; }
    
    internal sealed class GetWorkListByCandidateKeyHandler : IRequestHandler<GetWorkListByCandidateKey, WorkListNoteDTO>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public GetWorkListByCandidateKeyHandler(IDbContextFactory<ApplicationDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }
        
        public async Task<WorkListNoteDTO> Handle(GetWorkListByCandidateKey request, CancellationToken cancellationToken)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var entity = await context.WorkListNote
                .TagWith(nameof(GetWorkListByCandidateKey))
                .TagWithCallSiteSafely()
                .Include(e => e.Note)
                .Where(e => e.WorkListId == request.WorkListId 
                        && e.NoteId == request.NoteId)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity is null)
                throw new EntityNotFoundException(nameof(WorkListNote), 
                    new { request.WorkListId, request.NoteId });
            
            if (entity.Note is null)
                throw new EntityNotFoundException(nameof(Note), request.NoteId);

            return _mapper.Map<WorkListNoteDTO>(entity);
        }
    }
}