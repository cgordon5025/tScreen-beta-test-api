using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Admin.Models;
using AutoMapper;
using Core;
using Core.Extensions;
using Data;
using Domain.Entities;
using Domain.Entities.App;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.Session.Commands;

public class CloseSession : IRequest<SessionDTO>
{
    public Guid SessionId { get; init; }
    
    internal sealed class StopSessionHandler : IRequestHandler<CloseSession, SessionDTO>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IDateTime _dateTime;
        private readonly IMapper _mapper;

        public StopSessionHandler(IDbContextFactory<ApplicationDbContext> contextFactory, IDateTime dateTime, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _dateTime = dateTime;
            _mapper = mapper;
        }
        
        public async Task<SessionDTO> Handle(CloseSession request, CancellationToken cancellationToken)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(CancellationToken.None);

            var entity = await context.AppSessions
                .TagWith(nameof(CloseSession))
                .TagWithCallSiteSafely()
                .Where(e => e.Id == request.SessionId)
                .FirstOrDefaultAsync(CancellationToken.None);

            if (entity is null)
                throw new EntityNotFoundException(nameof(Session), request.SessionId);

            if (entity.FinishedAt is not null)
                throw new EntityCompleteException("Session already closed", entity.FinishedAt);

            entity.Checkpoint = SessionCheckpoints.AppClosed;
            entity.FinishedAt = _dateTime.NowUtc();
            entity.UpdatedAt = _dateTime.NowUtc();

            var data = Utility.SerializeObject(
                new HistoryCheckpointData { Label = SessionCheckpoints.AppClosed, Value = DateTime.UtcNow });

            entity.HistorySessions.Add(new HistorySession
            {
                SessionId = entity.Id,
                History = new History
                {
                    PersonId = entity.PersonId,
                    LocationId = entity.LocationId,
                    Type = HistoryTypes.SessionCheckpoint,
                    Data = data
                }
            });

            await context.SaveChangesAsync(CancellationToken.None);

            return _mapper.Map<SessionDTO>(entity);
        }
    }
}