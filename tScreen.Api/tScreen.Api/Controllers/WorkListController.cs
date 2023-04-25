using System.Linq;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Events;
using Application.Features.Admin.Session.Queries;
using Application.Features.Admin.WorkList.Commands;
using FluentValidation;
using GraphQl.GraphQl.Validators;
using GraphQl.Models;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GraphQl.Controllers;

[ApiController, Route("/api/worklist")]
public class WorkListController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidateService _validateService;
    private readonly IReportService _reportService;

    public WorkListController(IMediator mediator, IValidateService validateService, IReportService reportService)
    {
        _mediator = mediator;
        _validateService = validateService;
        _reportService = reportService;
    }
    
    [HttpPost("add")]
    public async Task<IActionResult> AddWorkLists([FromBody] AddWorkListsRequest requestModel)
    {
        var validator = new InlineValidator<AddWorkListsRequest>();
        
        validator
            .RuleFor(e => e.SessionId)
            .MustBeNonEmptyGuid();

        var errors = _validateService.ValidateModel(requestModel, validator);

        if (errors.Any())
            return BadRequest(new {
                Message = "Cannot create worklist because input data is invalid",
                Errors = errors
            });
        
        try
        { 
            await _reportService.GenerateSessionReport(requestModel.SessionId);
            
            var sessionDTO = await _mediator.Send(new GetSessionById
            {
                SessionId = requestModel.SessionId,
                ShouldThrow = true
            });
            
            await _mediator.Send(new AddWorkLists
            {
                SessionId = sessionDTO.Id, 
                LocationId = sessionDTO.LocationId
            });
        }
        catch (EntityNotFoundException)
        {
            return NotFound("Session doesn't exist");
        }
        catch (EntityIncompleteException)
        {
            return BadRequest("Cannot create worklist because record minimum requirements are not met");
        }
        catch (EntityCompleteException)
        {
            return BadRequest("Work lists already created for session");
        }
        
        return Ok();
    }

    [HttpPost("notify")]
    public async Task<IActionResult> NotifySessionWorklistPersons(
        [FromBody] NotifyWorklistAssociatedPersonsRequest requestModel)
    {
        var validator = new InlineValidator<NotifyWorklistAssociatedPersonsRequest>();
        
        validator
            .RuleFor(e => e.SessionId)
            .MustBeNonEmptyGuid();
        
        var errors = _validateService.ValidateModel(requestModel, validator);

        if (errors.Any())
            return BadRequest(new {
                Message = "Cannot create worklist because input data is invalid",
                Errors = errors
            });

        // ReSharper disable once InconsistentNaming
        var workListDTOs = await _mediator
            .Send(new GetWorkListsBySessionId { SessionId = requestModel.SessionId});
        
        await _mediator.Publish(new WorkListCreatedEvent(workListDTOs));

        return Ok();
    }
    
}