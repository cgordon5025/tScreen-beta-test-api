using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Events;
using Core.Settings;
using Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Features.Admin.WorkList.Notifications;

public class CreatedWorklistNotification : INotificationHandler<WorkListCreatedEvent>
{
    private readonly ILogger<CreatedWorklistNotification> _logger;
    private readonly IEmailService _emailService;
    private readonly UserManager<User> _userManager;
    private readonly TweenScreenClientSettings _clientSettings;

    public CreatedWorklistNotification(ILogger<CreatedWorklistNotification> logger, IEmailService emailService,
        UserManager<User> userManager, TweenScreenClientSettings clientSettings)
    {
        _logger = logger;
        _emailService = emailService;
        _userManager = userManager;
        _clientSettings = clientSettings;
    }

    public async Task Handle(WorkListCreatedEvent notification, CancellationToken cancellationToken)
    {
        foreach (var worklist in notification.Entities)
        {
            var person = worklist.Person;
            if (person is null)
            {
                _logger.LogCritical("WorkList missing Person object associated with PersonId {PersonId}", worklist.PersonId);
                continue;
            }

            var identity = await _userManager.FindByIdAsync(person.Id.ToString());
            if (identity is null)
            {
                _logger.LogCritical("Cannot find user record with ID {Id}", person.IdentityId);
                continue;
            }

            var fullName = $"{person.FirstName} {person.LastName}";

            _logger.LogInformation("Sending worklist email to {Email}", identity.Email);

            await _emailService.SendAsync(EmailTemplates.WorklistCreated, new Email<ReportReadyEmailData>()
            {
                To = new DataAddress(identity.Email, fullName),
                Subject = "fT Tween Screen Report Ready",
                Data = new ReportReadyEmailData
                {
                    WebClientUrl = _clientSettings.BaseUrl
                }
            });
        }
    }
}

public class ReportReadyEmailData : IEmailData
{
    public string Fullname { get; init; } = null!;
    public DateTime CurrentDate { get; init; }
    public string WebClientUrl { get; set; } = null!;
}