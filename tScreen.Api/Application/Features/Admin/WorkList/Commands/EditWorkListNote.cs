using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Admin.Models;
using AutoMapper;
using Core.Extensions;
using Data;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.WorkList.Commands;

public class EditWorkListNote : IRequest<WorkListNoteDTO>
{
    public WorkListNoteDTO? WorkListNoteDTO { get; init; }

    internal sealed class EditWorkListNoteHandler : IRequestHandler<EditWorkListNote, WorkListNoteDTO>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IDateTime _dateTime;
        private readonly IMapper _mapper;

        public EditWorkListNoteHandler(IDbContextFactory<ApplicationDbContext> contextFactory, IDateTime dateTime, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _dateTime = dateTime;
            _mapper = mapper;
        }
        
        public async Task<WorkListNoteDTO> Handle(EditWorkListNote request, CancellationToken cancellationToken)
        {
            if (request.WorkListNoteDTO is null)
                throw new NullReferenceException(nameof(request.WorkListNoteDTO));
            
            await using var context = await _contextFactory.CreateDbContextAsync(CancellationToken.None);

            var entity = await context.WorkListNote
                .TagWith(nameof(EditWorkListNote))
                .TagWithCallSiteSafely()
                .Include(e => e.Note)
                .Where(e => e.WorkListId == request.WorkListNoteDTO.WorkListId)
                .Where(e => e.NoteId == request.WorkListNoteDTO.NoteId)
                .FirstOrDefaultAsync(CancellationToken.None);

            if (entity is null)
                throw new EntityNotFoundException(nameof(WorkListNote), 
                    new { request.WorkListNoteDTO.WorkListId, request.WorkListNoteDTO.NoteId });

            entity = _mapper.Map(request.WorkListNoteDTO, entity);

            if (entity.Note is null)
                throw new Exception("The Note record for WorkListNote is unexpectedly missing");
            
            entity.Note.UpdatedAt = _dateTime.NowUtc();
            
            await context.SaveChangesAsync(CancellationToken.None);

            return _mapper.Map<WorkListNoteDTO>(entity);
        }
    }
}