using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Events;
using Core.Settings;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Admin.Person.Notifications;

public class EmailNewAccountNotification : INotificationHandler<PersonCreatedEvent>
{
    private readonly ILogger<EmailNewAccountNotification> _logger;
    private readonly IEmailService _emailService;
    private readonly TweenScreenClientSettings _clientSettings;

    public EmailNewAccountNotification(ILogger<EmailNewAccountNotification> logger, IEmailService emailService, 
        TweenScreenClientSettings clientSettings)
    {
        _logger = logger;
        _emailService = emailService;
        _clientSettings = clientSettings;
    }
    
    public Task Handle(PersonCreatedEvent notification, CancellationToken cancellationToken)
    {
        var fullname = $"{notification.Entity.FirstName} {notification.Entity.LastName}";
        
        _logger.LogInformation("Sending new account email to {Email}", notification.Email);
        
        _emailService.SendAsync(EmailTemplates.NewAccount, new Email<NewAccountData>()
        {
            To = new DataAddress(notification.Email, fullname),
            Subject = "Welcome!",
            Data = new NewAccountData
            {
                Id = notification.Entity.Id,
                Fullname = fullname,
                Email = notification.Email,
                Password = notification?.Password ?? "******",
                ClientUrl = _clientSettings.BaseUrl
            }
        });
        
        return Task.CompletedTask;
    }

    public class NewAccountData : IEmailData
    {
        public Guid Id { get; init; }
        public string Fullname { get; init; } = null!;
        public string Email { get; init; }
        public string Password { get; init; } = null!;
        public string ClientUrl { get; init; } = null!;
        
        public DateTime CurrentDate { get; init; } = DateTime.UtcNow;
    }
}