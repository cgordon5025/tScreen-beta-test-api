using System;
using Domain.Entities;

namespace Application.Events;

public class StudentCreatedEvent : BaseEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public Student Entity { get; }

    public StudentCreatedEvent(Student entity)
    {
        Entity = entity;
    }
}