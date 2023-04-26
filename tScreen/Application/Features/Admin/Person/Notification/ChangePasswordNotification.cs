using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Admin.Person.Notifications;

public class ChangePasswordNotification : INotificationHandler<ChangedPasswordEvent>
{
    private readonly ILogger<ChangePasswordNotification> _logger;
    private readonly IEmailService _emailService;

    public ChangePasswordNotification(ILogger<ChangePasswordNotification> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public Task Handle(ChangedPasswordEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending changed password email to {Email}", notification.Email);

        _emailService.SendAsync(EmailTemplates.ChangePassword, new Email<ChangePasswordEmailData>()
        {
            To = new DataAddress(notification.Email, notification.Fullname),
            Subject = "Your TweenScreen application password was recently changed",
            Data = new ChangePasswordEmailData
            {
                Fullname = notification.Fullname,
            }
        });

        return Task.CompletedTask;
    }

    public class ChangePasswordEmailData : IEmailData
    {
        public string Fullname { get; init; } = null!;
        public DateTime CurrentDate { get; init; } = DateTime.UtcNow;
    }
}