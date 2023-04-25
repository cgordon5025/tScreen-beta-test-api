using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Admin.Models;
using Application.Features.Admin.Session.Commands;
using Application.Features.Admin.Session.Queries;
using Application.Features.Admin.WorkList.Commands;
using Core;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GraphQl.Controllers;

[ApiController, Route("api/report")]
public class ReportController : Controller
{
    private readonly IReportService _reportService;
    private readonly IQueueService _queueService;
    private readonly IMediator _mediator;
    private readonly ILogger<ReportController> _logger;

    public ReportController(IReportService reportService, IQueueService queueService, 
        IMediator mediator, ILogger<ReportController> logger)
    {
        _reportService = reportService;
        _queueService = queueService;
        _mediator = mediator;
        _logger = logger;
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateReport([FromBody] CreateReportRequest requestModel)
    {
        var sessionDTO = await _mediator.Send(new GetSessionById
        {
            SessionId = requestModel.SessionId,
            ShouldThrow = false,
        });

        if (sessionDTO == null)
            return NotFound();
        
        // Put request into background thread so we can immediately return report 
        // to the requesting client, their request has been accepted.
        try
        {
            await _mediator.Send(new AddWorkLists
            {
                SessionId = sessionDTO.Id,
                LocationId = sessionDTO.LocationId
            });
        }
        catch (EntityCompleteException ex)
        {
            _logger.LogWarning("Worklist already created for session {SessionId}", requestModel.SessionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create report exception: {Message}", ex.Message);
            if (ex is EntityNotFoundException or EntityIncompleteException)
                throw;
        }
        
        var report = await _reportService.GenerateSessionReport(requestModel.SessionId);

        var data = new SessionDataDTO()
        {
            Tags = new SessionDataTagDTO() {
                Ace = report.ReportTags
                    .Where(x => x.Type == "ACE")
                    .Select(x => x.Name)
                    .OrderBy(x => x)
                    .ToArray(),
                Pce = report.ReportTags
                    .Where(x => x.Type == "PCE")
                    .Select(x => x.Name)
                    .OrderBy(x => x)
                    .ToArray()
            }
        };

        sessionDTO.Data = (object?) data;        

        await _mediator.Send(new EditSession
        {
            SessionDTO = sessionDTO
        });
        
        // After report is created, we're ready to notify all interested parties (teachers)
        // We do this by adding a message the work list queue
        var message = Utility.SerializeObject(new { requestModel.SessionId });
        await _queueService.SendMessageAsync(message, StorageQueues.CreateWorkListQueue);
            
        return Accepted();
    }
    
    public class CreateReportRequest
    {
        [Required]
        public Guid SessionId { get; set; }
    }
}