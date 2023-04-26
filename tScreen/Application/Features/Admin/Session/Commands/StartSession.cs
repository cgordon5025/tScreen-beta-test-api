using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
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

public class StartSession : IRequest<SessionDTO>
{
    public SessionDTO? SessionDTO { get; init; }
    internal sealed class StartSessionHandler : IRequestHandler<StartSession, SessionDTO>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public StartSessionHandler(IDbContextFactory<ApplicationDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<SessionDTO> Handle(StartSession request, CancellationToken cancellationToken)
        {
            if (request.SessionDTO == null)
                throw new NullReferenceException(nameof(request.SessionDTO));

            await using var context = await _contextFactory.CreateDbContextAsync(CancellationToken.None);

            var student = await context.Student
                .TagWith(nameof(StartSession))
                .TagWithCallSiteSafely()
                .Where(e => e.Id == request.SessionDTO.StudentId)
                .FirstOrDefaultAsync(CancellationToken.None);

            if (student is null)
                throw new EntityNotFoundException(nameof(Domain.Entities.Student), request.SessionDTO.StudentId);

            var entity = _mapper.Map<Domain.Entities.App.Session>(request.SessionDTO);

            // Cannot have a partial session if student hasn't provided the minimum required
            // in a previous session. Ensure session is type "full" if true
            if (entity.Type == SessionTypes.Partial && !student.MinimumProvided)
                entity.Type = SessionTypes.Full;

            // Code used for distributed invites
            entity.Code = Generate.CryptographicRandomString(32, 10);
            entity.Status = SessionStatus.Incomplete;

            var data = Utility.SerializeObject(
                    new HistoryCheckpointData { Label = SessionCheckpoints.Initiated, Value = DateTime.UtcNow });

            entity.HistorySessions.Add(new HistorySession
            {
                SessionId = entity.Id,
                History = new History
                {
                    PersonId = entity.PersonId,
                    LocationId = request.SessionDTO.LocationId,
                    Type = HistoryTypes.SessionCheckpoint,
                    Data = data
                }
            });

            await context.AddAsync(entity, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);

            return _mapper.Map<SessionDTO>(entity);
        }
    }
}