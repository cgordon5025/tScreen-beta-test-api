using System;
using MediatR;

namespace Application.Events;

public class BaseEvent : INotification
{
    public DateTimeOffset ZoneCreatedAt { get; set; } = DateTimeOffset.UtcNow;
}