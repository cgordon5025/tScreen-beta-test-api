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

namespace Application.Features.Admin.WorkList.Commands;

public class AddWorkListNote : IRequest<WorkListNoteDTO>
{
    public WorkListNoteDTO? WorkListNoteDTO { get; set; }

    internal sealed class AddWorkListNoteHandler : IRequestHandler<AddWorkListNote, WorkListNoteDTO>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public AddWorkListNoteHandler(IDbContextFactory<ApplicationDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<WorkListNoteDTO> Handle(AddWorkListNote request, CancellationToken cancellationToken)
        {
            if (request.WorkListNoteDTO is null)
                throw new NullReferenceException(nameof(request.WorkListNoteDTO));

            await using var context = await _contextFactory.CreateDbContextAsync(CancellationToken.None);

            var workList = await context.WorkList
                .TagWith(nameof(AddWorkListNote))
                .TagWithCallSiteSafely()
                .Where(e => e.Id == request.WorkListNoteDTO.WorkListId)
                .FirstOrDefaultAsync(CancellationToken.None);

            if (workList is null)
                throw new EntityNotFoundException(nameof(Domain.Entities.WorkList),
                    request.WorkListNoteDTO.WorkListId);

            var entity = _mapper.Map<WorkListNote>(request.WorkListNoteDTO);
            await context.WorkListNote.AddAsync(entity, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);

            return _mapper.Map<WorkListNoteDTO>(entity);
        }
    }
}