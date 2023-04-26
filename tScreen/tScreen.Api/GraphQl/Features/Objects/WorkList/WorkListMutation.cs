using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Admin.Models;
using Application.Features.Admin.WorkList.Commands;
using Application.Features.Admin.WorkList.Queries;
using AutoMapper;
using GraphQl.GraphQl.Attributes;
using GraphQl.GraphQl.Features.Objects.WorkList.Inputs;
using GraphQl.GraphQl.Features.Objects.WorkList.Results;
using GraphQl.GraphQl.Interfaces;
using GraphQl.GraphQl.Models;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using MediatR;

// ReSharper disable ClassNeverInstantiated.Global

namespace GraphQl.GraphQl.Features.Objects.WorkList;

[ExtendObjectType("Mutation"), Authorize]
public class WorkListMutation
{
    /// <summary>
    /// Create work lists for teachers associated with a student. Mutation can only be used
    /// once a session has a status of Pending. 
    /// </summary>
    public async Task<IWorkListResult> AddWorkLists(
        AddWorkListsInput input,
        [CurrentPersonContext] CurrentPersonContext personContext,
        [ReferenceCode] string referenceCode,
        [Service] IValidateService validateService,
        [Service] IMediator mediator)
    {
        // ReSharper disable once InconsistentNaming
        try
        {
            var errors = validateService.ValidateModel(input, new AddWorkListValidator());

            if (errors.Any())
                return new ValidationError
                {
                    Message = "Cannot create worklist because input data is invalid",
                    ReferenceCode = referenceCode,
                    Errors = errors
                };

            await mediator.Send(new AddWorkLists
            {
                SessionId = input.SessionId,
                LocationId = personContext.LocationId
            });

            return new WorkListsCreatedResult
            {
                Message = "Work lists created",
                Code = "Success"
            };
        }
        catch (EntityNotFoundException)
        {
            return new SessionNotFoundError
            {
                Message = "Session doesn't exist",
                Id = input.SessionId.ToString()
            };
        }
        catch (EntityIncompleteException ex)
        {
            return new SessionMinimumRequirementError
            {
                Message = "Cannot create worklist because record minimum requirements are not met",
                CurrentStatus = ex.CurrentStatus,
                ExpectedStatus = ex.ExpectedStatus
            };
        }
        catch (EntityCompleteException ex)
        {
            return new WorkListsAlreadyAddedError
            {
                Message = "Work lists already created for session",
                ChangedAt = ex.CompleteAt
            };
        }
    }

    /// <summary>
    /// Change the worklist status to "Read"
    /// </summary>
    /// <returns></returns>
    public async Task<IWorkListResult> MarkWorkListAsRead(MarkWorkList input, [Service] IMediator mediator)
    {
        try
        {
            var workListDTO = await mediator.Send(new GetWorkListById { Id = input.Id });
            workListDTO.Status = "Read";

            var worklist = await mediator.Send(new EditWorkList { WorkListDTO = workListDTO });

            return new MarkWorkListResult
            {
                Message = "Worklist marked as read",
                Code = "Success"
            };
        }
        catch (EntityNotFoundException ex)
        {
            return new WorkListNotFoundError
            {
                Message = "Worklist doesn't exist",
                Id = ex.Id.ToString() ?? string.Empty
            };
        }
    }

    /// <summary>
    /// Add a note to a specific work list
    /// </summary>
    public async Task<IWorkListResult> AddWorkListNote(
        AddWorkListNoteInput input,
        [ReferenceCode] string referenceCode,
        [Service] IValidateService validateService,
        [Service] IMediator mediator,
        [Service] IMapper mapper)
    {
        try
        {
            var errors = validateService.ValidateModel(input, new AddWorkListNoteValidator());

            if (errors.Any())
                return new ValidationError
                {
                    Message = "Cannot add worklist note because input data is invalid",
                    ReferenceCode = referenceCode,
                    Errors = errors
                };

            var workListNoteDTO = await mediator.Send(new AddWorkListNote
            {
                WorkListNoteDTO = new WorkListNoteDTO
                {
                    WorkListId = input.WorkListId,
                    Note = new NoteDTO
                    {
                        Type = input.Type.ToString(),
                        Body = input.Body,
                        Data = input.Data
                    }
                }
            });

            return mapper.Map<Models.WorkListNote>(workListNoteDTO);
        }
        catch (EntityNotFoundException ex)
        {
            return new WorkListNotFoundError
            {
                Message = "Worklist doesn't exist",
                Id = ex.Id.ToString() ?? string.Empty
            };
        }
    }

    /// <summary>
    /// Edit a specific worklist note
    /// </summary>
    public async Task<IWorkListResult> EditWorklistNote(
        EditWorkListNoteInput input,
        [Service] IMediator mediator,
        [Service] IMapper mapper)
    {
        try
        {
            var workListNoteDTO = await mediator.Send(new GetWorkListByCandidateKey
            {
                WorkListId = input.WorkListId,
                NoteId = input.Id
            });

            // If note is not found an EntityNotFound exception is thrown so we can safely
            // suppress null warnings 
            workListNoteDTO.Note!.Type = input.Type.ToString();
            workListNoteDTO.Note!.Body = input.Body;
            workListNoteDTO.Note!.Data = input.Data;

            workListNoteDTO = await mediator.Send(new EditWorkListNote { WorkListNoteDTO = workListNoteDTO });

            return mapper.Map<Models.WorkListNote>(workListNoteDTO);
        }
        catch (EntityNotFoundException)
        {
            return new WorkListNotFoundError
            {
                Message = "Either the work list or the note doesn't exist",
                Id = new { input.WorkListId, input.Id }.ToString() ?? string.Empty
            };
        }
    }
}