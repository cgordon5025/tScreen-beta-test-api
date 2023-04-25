using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Admin.Models;
using AutoMapper;
using Core.Extensions;
using Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.Session.Commands;

public class EditSession : IRequest<SessionDTO>
{
    public SessionDTO? SessionDTO { get; set; }
    
    internal sealed class EditSessionHandler : IRequestHandler<EditSession, SessionDTO>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IDateTime _dateTime;
        private readonly IMapper _mapper;

        public EditSessionHandler(IDbContextFactory<ApplicationDbContext> contextFactory, IDateTime dateTime, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _dateTime = dateTime;
            _mapper = mapper;
        }
        
        public async Task<SessionDTO> Handle(EditSession request, CancellationToken cancellationToken)
        {
            if (request.SessionDTO == null)
                throw new NullReferenceException(nameof(request.SessionDTO));
                
            await using var context = await _contextFactory.CreateDbContextAsync(CancellationToken.None);

            var entity = await context.AppSessions
                .TagWith(nameof(EditSession))
                .TagWithCallSiteSafely()
                .Where(e => e.Id == request.SessionDTO.Id)
                .FirstOrDefaultAsync(CancellationToken.None);

            if (entity == null)
                throw new NullReferenceException(nameof(request.SessionDTO));
            
            entity = _mapper.Map(request.SessionDTO, entity);
            entity.UpdatedAt = _dateTime.NowUtc();
            
            await context.SaveChangesAsync(CancellationToken.None);

            return _mapper.Map<SessionDTO>(entity);
        }
    }
}