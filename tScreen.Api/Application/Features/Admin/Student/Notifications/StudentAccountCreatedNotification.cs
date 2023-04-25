using System.Threading;
using System.Threading.Tasks;
using Application.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Admin.Student.Notifications;

public class StudentAccountCreatedNotification : INotificationHandler<StudentCreatedEvent>
{
    private readonly ILogger<StudentAccountCreatedNotification> _logger;

    public StudentAccountCreatedNotification(ILogger<StudentAccountCreatedNotification> logger)
    {
        _logger = logger;
    }
    
    public Task Handle(StudentCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Created student {FullName} ({Id}) and associated with {Associations} association(s)",
            notification.Entity.FullName, notification.Entity.Id, notification.Entity.PersonStudents.Count);

        return Task.CompletedTask;
    }
}