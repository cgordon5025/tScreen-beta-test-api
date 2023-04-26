using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.App.Answer.Models;
using Application.Features.App.File.Commands;
using AutoMapper;
using Core;
using Core.Extensions;
using Data;
using Domain.Entities.App;
using GraphQl.GraphQl.DataLoaders;
using GraphQl.GraphQl.Models;
using GreenDonut;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using File = GraphQl.GraphQl.Models.File;

// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable ClassNeverInstantiated.Global

namespace GraphQl.GraphQl.Features.Objects.Session;

public class SessionType : ObjectType<Models.Session>
{
    protected override void Configure(IObjectTypeDescriptor<Models.Session> descriptor)
    {
        descriptor.Field(e => e.Student)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<SessionResolvers>(r =>
                r.GetStudentAsync(default!, default!, default))
            .Name("student");

        descriptor.Field(e => e.Person)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<SessionResolvers>(r =>
                r.GetPersonAsync(default!, default!, default))
            .Name("person");

        descriptor.Field(e => e.Adventure)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<SessionResolvers>(r =>
                r.GetAdventureAsync(default!, default!, default))
            .Name("adventure");

        descriptor.Field(e => e.Avatar)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<SessionResolvers>(r =>
                r.GetAvatarAsync(default!, default!, default))
            .Name("avatar");

        descriptor.Field(e => e.SessionNotes)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<SessionResolvers>(r =>
                r.GetNotesAsync(default!, default!, default!, default))
            .Name("notes");

        descriptor.Field(e => e.WorkLists)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<SessionResolvers>(r =>
                r.GetWorkLists(default!, default!, default!, default))
            .Name("workLists");

        descriptor.Field(e => e.Type)
            .ResolveWith<SessionResolvers>(r => r.GetNextCheckpoint(default!))
            .Name("nextCheckpoint");

        descriptor.Field(e => e.SessionSummary)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<SessionResolvers>(r => r.GetSessionSummary(default!, default!, default))
            .Name("summary");

        descriptor.Field(e => e.File)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<SessionResolvers>(r =>
                r.GetSessionFile(default!, default!, default!, default!))
            .Name("report");
    }

    private class SessionResolvers
    {
        public async Task<Models.Student> GetStudentAsync(
            [Parent] Models.Session session,
            StudentByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
            => await dataLoader.LoadAsync(session.StudentId, cancellationToken);

        public async Task<Models.Person> GetPersonAsync(
            [Parent] Models.Session session,
            PersonByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
            => await dataLoader.LoadAsync(session.PersonId, cancellationToken);

        public async Task<Models.Adventure?> GetAdventureAsync(
            [Parent] Models.Session session,
            AdventureByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
        {
            if (session.AdventureId is null)
                return null;

            return await dataLoader.LoadAsync(session.AdventureId ?? Guid.Empty, cancellationToken);
        }



        public async Task<Models.Avatar?> GetAvatarAsync(
            [Parent] Models.Session session,
            AvatarByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
        {
            if (session.AvatarId is null)
                return null;

            return await dataLoader.LoadAsync(session.AvatarId ?? Guid.Empty, cancellationToken);
        }

        public async Task<SessionSummary?> GetSessionSummary(
            [Parent] Models.Session session,
            [ScopedService] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var answers = await context.AppAnswers
                .TagWith(nameof(GetSessionSummary))
                .TagWithCallSiteSafely()
                .Where(e => e.SessionId == session.Id)
                .Where(e => new[]
                {
                    MappedQuestionIds.WhoDoYouLiveWithQuestionId,
                    MappedQuestionIds.WhoCanYouCountOnInTheFamilyQuestionId,
                    MappedQuestionIds.WhatAboutSiblingsQuestionId
                }.Contains(e.QuestionId))
                .ToListAsync(cancellationToken);

            var mappedAnswers = answers.ToDictionary(x => x.QuestionId);

            if (!answers.Any())
                return null;

            Domain.Entities.App.Answer? answer;
            var sessionSummary = new SessionSummary
            {
                Id = session.Id,
                Status = session.Status
            };

            if (mappedAnswers.ContainsKey(MappedQuestionIds.WhoDoYouLiveWithQuestionId))
            {
                answer = mappedAnswers[MappedQuestionIds.WhoDoYouLiveWithQuestionId];
                var answerPayload = Utility
                    .DeserializeObject<AnswerPayloadDTO<AnswerDataLivingSituationDTO>>(answer.Data);

                sessionSummary.LivingSituation = answerPayload.Data?.LivingSituation;
            }

            if (mappedAnswers.ContainsKey(MappedQuestionIds.WhatAboutSiblingsQuestionId))
            {
                answer = mappedAnswers[MappedQuestionIds.WhatAboutSiblingsQuestionId];
                var answerPayload = Utility
                    .DeserializeObject<AnswerPayloadDTO<IReadOnlyList<AnswerDataSiblingsDTO>>>(answer.Data);

                sessionSummary.NumberOfSiblings = answerPayload.Data?.Count ?? 0;
            }

            if (mappedAnswers.ContainsKey(MappedQuestionIds.WhoCanYouCountOnInTheFamilyQuestionId))
            {
                answer = mappedAnswers[MappedQuestionIds.WhoCanYouCountOnInTheFamilyQuestionId];
                var answerPayload = Utility.DeserializeObject<AnswerPayloadDTO<string[]>>(answer.Data);

                sessionSummary.PeopleCountedOn = answerPayload?.Data ?? Array.Empty<string>();
            }

            return sessionSummary;
        }

        public async Task<IEnumerable<Models.Note>> GetNotesAsync(
            [Parent] Models.Session session,
            [ScopedService] ApplicationDbContext context,
            NoteByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
        {
            var noteIds = await context.SessionNote
                .TagWith(nameof(GetNotesAsync))
                .TagWithCallSiteSafely()
                .Where(e => e.SessionId == session.Id)
                .Select(e => e.NoteId)
                .ToListAsync(cancellationToken);

            return await dataLoader.LoadAsync(noteIds, cancellationToken);
        }

        public async Task<IEnumerable<Models.WorkList>> GetWorkLists(
            [Parent] Models.Session session,
            [ScopedService] ApplicationDbContext context,
            WorkListByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
        {
            var workListIds = await context.WorkList
                .TagWith(nameof(GetWorkLists))
                .TagWithCallSiteSafely()
                .Where(e => e.SessionId == session.Id)
                .Select(e => e.Id)
                .ToListAsync(cancellationToken);

            return await dataLoader.LoadAsync(workListIds, cancellationToken);
        }

        public async Task<File?> GetSessionFile(
            [Parent] Models.Session session,
            [ScopedService] ApplicationDbContext context,
            [Service] IMapper mapper,
            CancellationToken cancellationToken)
        {
            var entity = await context.AppSessionFile
                .TagWith(nameof(GetSessionFile))
                .TagWithCallSiteSafely()
                .Where(e => e.SessionId == session.Id)
                .OrderByDescending(e => e.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            return mapper.Map<File>(entity);
        }

        public string GetNextCheckpoint([Parent] Models.Session session)
            => SessionCheckpoints.Next(session.Checkpoint);
    }
}