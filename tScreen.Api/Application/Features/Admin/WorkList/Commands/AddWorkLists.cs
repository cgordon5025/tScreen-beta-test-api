using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Admin.Models;
using AutoMapper;
using Core;
using Core.Extensions;
using Data;
using Domain.Entities;
using Domain.Entities.App;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.WorkList.Commands;

public class AddWorkLists : IRequest<IEnumerable<WorkListDTO>>
{
    public Guid SessionId { get; init; }
    public Guid LocationId { get; init; }

    internal sealed class AddWorkListHandler : IRequestHandler<AddWorkLists, IEnumerable<WorkListDTO>>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IQueueService _queueService;
        private readonly IDateTime _dateTime;
        private readonly IMapper _mapper;

        public AddWorkListHandler(IDbContextFactory<ApplicationDbContext> contextFactory, 
            IQueueService queueService, IDateTime dateTime, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _queueService = queueService;
            _dateTime = dateTime;
            _mapper = mapper;
        }
        
        public async Task<IEnumerable<WorkListDTO>> Handle(AddWorkLists request, CancellationToken cancellationToken)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(CancellationToken.None);
            var entity = await context.AppSessions
                .TagWith(nameof(AddWorkLists))
                .TagWithCallSiteSafely()
                .Include(e => e.Student)
                .Where(e => e.Id == request.SessionId)
                .FirstOrDefaultAsync(CancellationToken.None);

            if (entity is null)
                throw new EntityNotFoundException(nameof(Session), request.SessionId);

            if (entity.Status is SessionStatus.Complete)
                throw new EntityCompleteException("Already created work lists for this session", entity.UpdatedAt);
            
            if (entity.Status is not SessionStatus.Pending)
                throw new EntityIncompleteException(nameof(Session), entity.Id, 
                    entity.Status ?? "NULL", SessionStatus.Complete);
            
            var personIds = await context.PersonStudent
                .TagWith($"{nameof(AddWorkLists)}-PersonIds")
                .TagWithCallSiteSafely()
                .Where(e => e.StudentId == entity.StudentId)
                .Select(e => e.PersonId)
                .ToListAsync(CancellationToken.None);
            
            if (!personIds.Any())
                return new List<WorkListDTO>();

            var workLists = new List<Domain.Entities.WorkList>();
            var currentTime = _dateTime.NowUtc();

            var prevWorkLists = await context.WorkList
                .Where(e => e.SessionId == request.SessionId)
                .ToDictionaryAsync(e => e.PersonId, CancellationToken.None);
            
            foreach (var personId in personIds)
            {
                // Ensure that a person can only have a single worklist for a session
                // thus supporting idempotency
                if (prevWorkLists.ContainsKey(personId)) continue;

                var workList = new Domain.Entities.WorkList
                {
                    SessionId = entity.Id,
                    PersonId = personId, 
                    Type = "Default",
                    LocationId = request.LocationId,
                    CreatedAt = currentTime,
                    Status = WorkListStatus.Unread
                };

                workList.HistoryWorkLists.Add(new HistoryWorkList
                {
                    History = new History
                    {
                        LocationId = request.LocationId,
                        PersonId = personId,
                        Type = "WorkList.Added",
                        Data = Utility.SerializeObject(new
                        {
                            personId,
                            studentId = entity.StudentId,
                            adventureId = entity.AdventureId,
                            sessionId = entity.Id,
                            locationId = request.LocationId
                        }),
                        CreatedAt = currentTime
                    },
                    CreatedAt = currentTime
                });

                workLists.Add(workList);
            }

            entity.Status = SessionStatus.Complete;
            entity.UpdatedAt = currentTime;
            
            await context.WorkList.AddRangeAsync(workLists, cancellationToken);
            await context.SaveChangesAsync(CancellationToken.None);

            var message = Utility.SerializeObject(new { SessionId = entity.Id });
            await _queueService.SendMessageAsync(message, StorageQueues.CreateWorkListQueue);
                
            return _mapper.Map<IEnumerable<WorkListDTO>>(workLists);
        }
    }
}