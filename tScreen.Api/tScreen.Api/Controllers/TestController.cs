// This controller isn't accessible in production/release build
// #if DEBUG

using System;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Interfaces;
using Core;
using Core.Settings;
using Infrastructure.Services;
using IronPdf;
using Microsoft.AspNetCore.Mvc;

namespace GraphQl.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : Controller
{
    private readonly IReportService _reportService;
    private readonly IQueueService _queueService;
    private readonly IEmailService _emailService;
    private readonly EmailSettings _emailSettings;

    public TestController(IReportService reportService, IQueueService queueService, 
        IEmailService emailService, EmailSettings emailSettings)
    {
        _reportService = reportService;
        _queueService = queueService;
        _emailService = emailService;
        _emailSettings = emailSettings;
    }

    [HttpPost("report/assessment-generator")]
    public async Task<IActionResult> TestReportAssessmentGenerator([FromBody] SessionRequest requestModel)
    {
        var assessment = await _reportService.GenerateSessionReport(requestModel.SessionId);
        return Ok(assessment);
    }

    [HttpPost("create-worklist")]
    public async Task<IActionResult> TestCreateReportQueue()
    {
        var message = Utility.SerializeObject(new { SessionId = "E0B8433E-F36B-1410-8A53-00F2E42120C8" });
        await _queueService.SendMessageAsync(message, StorageQueues.CreateReportQueue);

        return Ok(message);
    }

    [HttpPost("email-probe")]
    public async Task<IActionResult> TestEmailService([FromBody] EmailProbeRequest requestModel)
    {
        var tag = requestModel.Tag ?? "Testing";
        var subject = requestModel.Subject ?? "Test probe mail";
        var body = requestModel.Body ??
                   "Just a test message sent to verify the email system is configured correctly and operational";
        
        var fromEmail = requestModel.FromEmail ?? _emailSettings.FromEmail;
        await _emailService.SendAsync(requestModel.ToEmail, fromEmail, subject, body, tag);
        
        return Ok();
    }

    public class SessionRequest
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Guid SessionId { get; set; }
    }

    public class EmailProbeRequest
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string ToEmail { get; set; } = null!;
        public string? FromEmail { get; set; } = null;
        public string? Subject { get; set; } = null;
        public string? Body { get; set; } = null;
        public string? Tag { get; set; } = null;
    }
}

//#endif