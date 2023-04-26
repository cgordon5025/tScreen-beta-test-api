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
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.WorkList.Commands;

public class EditWorkList : IRequest<WorkListDTO>
{
    public WorkListDTO WorkListDTO { get; set; }

    internal sealed class EditWorkListHandler : IRequestHandler<EditWorkList, WorkListDTO>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IDateTime _dateTime;
        private readonly IMapper _mapper;

        public EditWorkListHandler(IDbContextFactory<ApplicationDbContext> contextFactory, IDateTime dateTime, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _dateTime = dateTime;
            _mapper = mapper;
        }

        public async Task<WorkListDTO> Handle(EditWorkList request, CancellationToken cancellationToken)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(CancellationToken.None);

            var entity = await context.WorkList
                .TagWith(nameof(EditWorkList))
                .TagWithCallSiteSafely()
                .Where(e => e.Id == request.WorkListDTO.Id)
                .FirstOrDefaultAsync(CancellationToken.None);

            if (entity is null)
                throw new EntityNotFoundException(nameof(Domain.Entities.WorkList), request.WorkListDTO.Id);

            entity = _mapper.Map(request.WorkListDTO, entity);

            entity.UpdatedAt = _dateTime.NowUtc();

            await context.SaveChangesAsync(CancellationToken.None);

            return _mapper.Map<WorkListDTO>(entity);
        }
    }
}