using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Admin.Models;
using Application.Features.Admin.Session.Commands;
using Application.Features.Admin.Session.Queries;
using AutoMapper;
using GraphQl.GraphQl.Attributes;
using GraphQl.GraphQl.Features.Objects.Session.Inputs;
using GraphQl.GraphQl.Features.Objects.Session.Results;
using GraphQl.GraphQl.Interfaces;
using GraphQl.GraphQl.Models;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using MediatR;

// ReSharper disable ClassNeverInstantiated.Global

namespace GraphQl.GraphQl.Features.Objects.Session;

[ExtendObjectType("Mutation"), Authorize]
public class SessionMutation
{
    /// <summary>
    /// Start a session with checkpoint type "Initialized"
    /// </summary>
    public async Task<ISessionResult> BeginSession(
        StartSessionInput input,
        [CurrentPersonContext] CurrentPersonContext currentPersonContext,
        [ReferenceCode] string referenceCode,
        [Service] IValidateService validateService,
        [Service] IMediator mediator,
        [Service] IMapper mapper)
    {
        var errors = validateService.ValidateModel(input, new StartSessionValidator());

        if (errors.Any())
            return new ValidationError
            {
                Message = "Cannot create session because input data is invalid",
                ReferenceCode = referenceCode,
                Errors = errors
            };
        
        var sessionDTO = new SessionDTO
        {
            StudentId = input.StudentId,
            Type = input.Type.ToString(),
            PersonId = currentPersonContext.PersonId,
            LocationId = currentPersonContext.LocationId
        };

        sessionDTO = await mediator.Send(new StartSession { SessionDTO = sessionDTO });

        return mapper.Map<Models.Session>(sessionDTO);
    }

    public async Task<ISessionResult> EditSession(
        EditSessionInput input,
        [ReferenceCode] string referenceCode,
        [Service] IValidateService validateService,
        [Service] IMediator mediator,
        [Service] IMapper mapper)
    {
        var errors = validateService.ValidateModel(input, new EditSessionInputValidator());
        
        if (errors.Any())
            return new ValidationError
            {
                Message = "Cannot create session because input data is invalid",
                ReferenceCode = referenceCode,
                Errors = errors
            };

        
        var sessionDTO = await mediator.Send(new GetSessionById { SessionId = input.SessionId });

        sessionDTO.AdventureId = input.AdventureId;
        sessionDTO.AvatarId = input.AvatarId; 

        sessionDTO = await mediator.Send(new EditSession { SessionDTO = sessionDTO });

        return mapper.Map<Models.Session>(sessionDTO);
    }

    /// <summary>
    /// Advance a session checkpoint and log details. Some checkpoints 
    /// Advance checkpoint and log details for each checkpoint shifted. 
    /// </summary>
    public async Task<ISessionResult> ShiftSessionCheckpoint(
        ShiftSessionCheckpointInput input,
        [ReferenceCode] string referenceCode,
        [Service] IValidateService validateService,
        [Service] IMediator mediator,
        [Service] IMapper mapper)
    {
        var errors = validateService.ValidateModel(input, new ShiftSessionCheckpointValidator());

        if (errors.Any())
            return new ValidationError
            {
                Message = "Cannot shift session checkpoint because input data is invalid",
                ReferenceCode = referenceCode,
                Errors = errors
            };
        
        var answersDTO = input.Answers?
            .Select(x => new AnswerDTO()
             {
                 QuestionId = x.QuestionId,
                 Data = x.Data
             }).ToList() ??
             new List<AnswerDTO>();
        
        try
        {
            var sessionDTO = await mediator.Send(new ShiftSessionCheckpoint
            {
                SessionId = input.SessionId,
                AdventureId = input.AdventureId,
                AvatarId = input.AvatarId,
                Type = input.Type.ToString(),
                Answers = answersDTO,
                UsePreviousSession = input.UsePreviousSession ?? false
            });
                
            return mapper.Map<Models.Session>(sessionDTO);
        }
        catch (EntityDuplicateException ex)
        {
            return new DuplicateCheckpointError
            {
                Message = "Checkpoint for this session is already logged",
                Checkpoint = ex.Columns.ToString() ?? "",
                CreatedAt = ex.CreatedAt
            };
        }
        catch (EntityCompleteException ex)
        {
            return new SessionAlreadyClosedError
            {
                Message = "Session is already closed",
                ClosedAt = ex.CompleteAt
            };
        }
    }

    /// <summary>
    /// Stop session and set checkpoint to "AppClosed"
    /// </summary>
    public async Task<ISessionResult> CloseSession(
        CloseSessionInput input, 
        [ReferenceCode] string referenceCode,
        [Service] IValidateService validateService,
        [Service] IMediator mediator,
        [Service] IMapper mapper)
    {
        try
        {
            var errors = validateService.ValidateModel(input, new CloseSessionInputValidator());

            if (errors.Any())
                return new ValidationError
                {
                    Message = "Cannot close session because input data is invalid",
                    ReferenceCode = referenceCode,
                    Errors = errors
                };
            
            var sessionDTO = await mediator.Send(new CloseSession { SessionId = input.Id });

            return mapper.Map<Models.Session>(sessionDTO);
        }
        catch (EntityNotFoundException ex)
        {
            return new SessionNotFoundError
            {
                Message = "Session doesn't exist",
                Id = ex.Id.ToString()
            };
        }
        catch (EntityCompleteException ex)
        {
            return new SessionAlreadyClosedError
            {
                Message = "Session is already closed",
                ClosedAt = ex.CompleteAt
            };
        }
    }
}