using System;
using System.IO;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Models;
using Core.Settings;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.SendGrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailService> _logger;
    private readonly SendGridSettings _sendGrid;

    public EmailService(IServiceProvider serviceProvider, ILogger<EmailService> logger, SendGridSettings sendGrid)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _sendGrid = sendGrid;
    }

    public async Task SendAsync<T>(string template, Email<T> email, string tag = "general") where T : IEmailData
    {
        // Name represents the Email template name without the extensions 
        var fileName = template switch
        {
            EmailTemplates.NewAccount => "NewAccount",
            EmailTemplates.ResetPassword => "PasswordReset",
            EmailTemplates.ForgotPassword => "ForgotPassword",
            EmailTemplates.ChangePassword => "ChangePassword",
            EmailTemplates.WorklistCreated => "ReportReady",
            _ => throw new Exception("Template name not found")
        };

        var templatePath = Path.Combine(new[]
        {
            Directory.GetCurrentDirectory(),
            "Templates",
            "Emails",
            fileName
        });

        using var scope = _serviceProvider.CreateScope();
        var fluentEmail = scope.ServiceProvider.GetRequiredService<IFluentEmail>()
            .To(email.To.Address, email.To.Name)
            .Subject(email.Subject)
            .Tag(tag)
            .UsingTemplateFromFile($"{templatePath}.cshtml", email.Data)
            .PlaintextAlternativeUsingTemplateFromFile($"{templatePath}.txt.cshtml", email.Data);

        if (email.From is not null)
            fluentEmail.SetFrom(email.From.Address, email.From.Name);

        SendResponse response;
        if (string.IsNullOrWhiteSpace(_sendGrid.ApiKey))
        {
            response = await fluentEmail.SendAsync().ConfigureAwait(false);
        }
        else
        {
            var sendGrid = new SendGridSender(_sendGrid.ApiKey);
            response = await sendGrid.SendAsync(fluentEmail);
        }

        if (response.Successful)
        {
            _logger.LogInformation("Successfully sent email to {EmailAddress}", email.To.Address);
        }
        else
        {
            _logger.LogWarning("Failed to send email: {Message}", response.ErrorMessages);
        }
    }

    public async Task SendAsync(string to, string from, string subject, string body, string tag = "general")
    {
        _logMessage(to, from, subject, body);

        using var scope = _serviceProvider.CreateScope();
        var fluentEmail = scope.ServiceProvider.GetRequiredService<IFluentEmail>()
            .To(to)
            .Subject(subject)
            .Tag(tag)
            .Body(body);

        if (!string.IsNullOrWhiteSpace(from))
            fluentEmail.SetFrom(from);

        SendResponse response;
        if (string.IsNullOrWhiteSpace(_sendGrid.ApiKey))
        {
            response = await fluentEmail.SendAsync().ConfigureAwait(false);
        }
        else
        {
            var sendGrid = new SendGridSender(_sendGrid.ApiKey);
            response = await sendGrid.SendAsync(fluentEmail);
        }

        if (response.Successful)
        {
            _logger.LogInformation("Successfully sent email to {Email}", from);
        }
        else
        {
            _logger.LogWarning("Failed to send email: {Message}", response.ErrorMessages);
        }
    }

    private void _logMessage(string to, string from, string subject, string body)
    {
        _logger.LogInformation(@"
TO: {To}
FROM: {From}
SUBJECT: {Subject}
BODY: {Body}
        ", to, from, subject, body);
    }
}