using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Events;
using Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Admin.Person.Notifications;

public class ForgotPasswordNotification : INotificationHandler<ForgotPasswordEvent>
{
    private readonly ILogger<ForgotPasswordNotification> _logger;
    private readonly IEmailService _emailService;

    public ForgotPasswordNotification(ILogger<ForgotPasswordNotification> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }
    
    public Task Handle(ForgotPasswordEvent notification, CancellationToken cancellationToken)
    {
        var callbackUrl = $"{notification.ClientUrl}?token={notification.Token.ToBase64()}";
        
        _logger.LogInformation("Sending forgot password email to {Email}", notification.Email);
        
        _emailService.SendAsync(EmailTemplates.ForgotPassword, new Email<ForgotPasswordEmailData>()
        {
            To = new DataAddress(notification.Email, notification.Fullname),
            Subject = "Password reset for your TweenScreen application account",
            Data = new ForgotPasswordEmailData
            {
                Fullname = notification.Fullname,
                CallbackUrl = callbackUrl
            }
        });

        return Task.CompletedTask;
    }

    public class ForgotPasswordEmailData : IEmailData
    {
        public string Fullname { get; init; } = null!;
        public string CallbackUrl { get; init; } = null!;
        public DateTime CurrentDate { get; init; } = DateTime.UtcNow;
    }
}