using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Admin.Models;
using Application.Features.App.Answer.Models;
using AutoMapper;
using Core;
using Core.Extensions;
using Data;
using Domain.Entities;
using Domain.Entities.App;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Application.Features.Admin.Session.Commands;

public class ShiftSessionCheckpoint : IRequest<SessionDTO>
{
    public Guid SessionId { get; init; }
    public Guid? AdventureId { get; init; }
    public Guid? AvatarId { get; init; }
    public string Type { get; init; } = null!;
    public List<AnswerDTO> Answers { get; init; } = new ();

    public bool UsePreviousSession { get; init; } = false;

    internal sealed class ShiftSessionCheckpointHandler : IRequestHandler<ShiftSessionCheckpoint, SessionDTO>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IQueueService _queueService;
        private readonly IDateTime _dateTime;
        private readonly IMapper _mapper;
        
        private readonly List<string> _allowedTypes = new ()
        {
            SessionCheckpoints.AvatarSelected,
            SessionCheckpoints.AuthenticationSuccess,
            SessionCheckpoints.EnvironmentQuestions,
            SessionCheckpoints.PersonalQuestions,
            SessionCheckpoints.AdventureSelected,
            SessionCheckpoints.AdventureComplete,
            SessionCheckpoints.AppClosed
        };
        
        public ShiftSessionCheckpointHandler(IDbContextFactory<ApplicationDbContext> contextFactory,
            IQueueService queueService, IDateTime dateTime, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _queueService = queueService;
            _dateTime = dateTime;
            _mapper = mapper; 
        }

        public async Task<SessionDTO> Handle(ShiftSessionCheckpoint request, CancellationToken cancellationToken)
        {
            if (request.SessionId == Guid.Empty)
                throw new EntityIdRequiredException(nameof(request.SessionId));

            await using var context = await _contextFactory.CreateDbContextAsync(CancellationToken.None);
            await using var transaction = await context.Database.BeginTransactionAsync(CancellationToken.None);

            var committedTransaction = false;
            
            try
            {
                var entity = await context.AppSessions
                    .TagWith(nameof(ShiftSessionCheckpoint))
                    .TagWithCallSiteSafely()
                    .Include(e => e.Student)
                    .Include(e => e.HistorySessions)
                    .ThenInclude(e => e.History)
                    .Where(e => e.Id == request.SessionId)
                    .FirstOrDefaultAsync(CancellationToken.None);

                if (entity is null)
                    throw new EntityNotFoundException(nameof(Session), request.SessionId);
            
                if (entity.FinishedAt is not null)
                    throw new EntityCompleteException("Session already closed", entity.FinishedAt);
            
                if (entity.HistorySessions.Any())
                    foreach (var historySession in entity.HistorySessions)
                    {
                        if (historySession.History?.Data is null 
                            || historySession.History?.Type != HistoryTypes.SessionCheckpoint) continue;

                        var checkpointData = JsonConvert
                            .DeserializeObject<HistoryCheckpointData>(historySession.History.Data);

                        if (request.Type == checkpointData?.Label)
                            throw new EntityDuplicateException(nameof(History), new { request.Type }, 
                                historySession.History.CreatedAt);
                    }

                if (!_allowedTypes.Contains(request.Type))
                    throw new ArgumentOutOfRangeException($"Type {request.Type} is not a valid session checkpoint type");

                var currentDateTime = _dateTime.NowUtc();
            
                // Once the student/player completes the environment question session
                // we set a flag in the student table which is used to determine how
                // subsequent evaluation flow
                if (request.Type == SessionCheckpoints.EnvironmentQuestions && entity.Student?.MinimumProvided == false)
                {
                    entity.Student.MinimumProvided = true;
                    entity.Student.UpdatedAt = currentDateTime;
                }

                var data = Utility.SerializeObject(
                    new HistoryCheckpointData { Label = request.Type, Value = currentDateTime });

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

                entity.Checkpoint = request.Type;
                
                // Load questions from previous session
                var previousAnswers = new List<Answer>();
                if (request.Type == SessionCheckpoints.EnvironmentQuestions && request.UsePreviousSession)
                    previousAnswers = await GetAnswersFromPreviousSession(request, context);
                
                
                // TODO -- Maybe improve answer validation?
                // Currently we don't check if the question belongs to a certain check point
                // So in theory a question could be answered at any checkpoint, multiple times.
                //
                // Check if we have previous answers, if so we'll merge them with the newly
                // provider answers. If not, just use the new answers.
                var newAnswers = await HandleAnswers(request, context);
                var allAnswers = previousAnswers.Any() 
                    ? previousAnswers.Concat(newAnswers).ToList()
                    : newAnswers;
                
                // Ensure recent answers surface to top. 
                // De-dup answers encase there's duplicates (shouldn't be)
                // Order answers by the natural order the questions were asked.
                entity.Answers = allAnswers
                    .OrderByDescending(e => e.CreatedAt)
                    .DistinctBy(e => e.QuestionId)
                    .ToList()
                    .OrderBy(e => e.Question?.Position)
                    .ToList();

                var sendMessage = false;
                switch (request.Type)
                {
                    case SessionCheckpoints.AvatarSelected:
                        entity.AvatarId = request.AvatarId;
                        entity.UpdatedAt = currentDateTime;
                        break;
                
                    case SessionCheckpoints.AdventureSelected:
                        entity.AdventureId = request.AdventureId;
                        entity.UpdatedAt = currentDateTime;
                        break;
                
                    case SessionCheckpoints.AdventureComplete:
                    {
                        entity.FinishedAt = currentDateTime;
                        entity.Status = SessionStatus.Pending;
                        entity.UpdatedAt = currentDateTime;
                        sendMessage = true;
                        break;
                    }
                }

                await context.SaveChangesAsync(CancellationToken.None);
                await transaction.CommitAsync(CancellationToken.None);
                committedTransaction = true;

                // If checkpoint AdventureComplete is complete then we'll need to start
                // the report generation and notification process. Note: we send the 
                // message after we save the session and commit the DB transaction
                if (sendMessage)
                {
                    var message = Utility.SerializeObject(new { SessionId = entity.Id });
                    await _queueService.SendMessageAsync(message, StorageQueues.CreateReportQueue);
                }
                
                return _mapper.Map<SessionDTO>(entity);
            }
            catch (Exception)
            {
                if (!committedTransaction)
                    await transaction.RollbackAsync(CancellationToken.None);
                
                throw;
            }
        }

        private async Task<List<Answer>> GetAnswersFromPreviousSession(ShiftSessionCheckpoint request,
            ApplicationDbContext context)
        {
            if (!request.UsePreviousSession) return new List<Answer>();

            var cherryPickedQuestions = new[]
            {
                MappedQuestionIds.AllFamilyMembersInLifeQuestionId,
                MappedQuestionIds.WhoDoYouLiveWithQuestionId,
                MappedQuestionIds.WhatAboutSiblingsQuestionId,
                MappedQuestionIds.ParentDescribeTheirMoodQuestionId,
                MappedQuestionIds.DescribeThisPersonQuestionId,
                // MappedQuestionIds.ExperiencedDeathInTheFamilyQuestionId,
                MappedQuestionIds.WhoCanYouCountOnInTheFamilyQuestionId
            };
            
            // Get previous successfully completed session with answers
            var previousSessionId = await context.AppSessions
                .Include(e => e.Answers)
                .Where(e => e.Checkpoint == SessionCheckpoints.AdventureComplete)
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => e.Id)
                .FirstOrDefaultAsync();
            
            // Cherry pick answers
            var cherryPickedAnswers = await context.AppAnswers
                .Where(e => e.SessionId == previousSessionId)
                .Where(e => cherryPickedQuestions.Contains(e.QuestionId))
                .ToListAsync(CancellationToken.None);
            
            // Ensure no duplicates (de-dup after pulling results from the DB)
            var distinctResults = cherryPickedAnswers
                .DistinctBy(e => e.QuestionId);
            
            // Clone answers and return array
            return distinctResults.ToList();
        }

        private async Task<List<Answer>> HandleAnswers(ShiftSessionCheckpoint request,
            ApplicationDbContext context)
        {
            if (!request.Answers.Any()) return new List<Answer>();
            
            var answers = new List<Answer>();
            foreach (var answer in request.Answers)
            {
                if (answer is null)
                    throw new NullReferenceException(nameof(answer));

                var entity = _mapper.Map<Answer>(answer);
                entity.SessionId = request.SessionId;
                await context.AppAnswers.AddAsync(entity, CancellationToken.None);
                answers.Add(entity);
            }

            await context.SaveChangesAsync(CancellationToken.None);
            return answers;
        }
    }
}