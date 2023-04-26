using System;
using Application.Common.Interfaces;

namespace Application.Events;

public class EntityCreatedEvent<T> : IEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Type { get; }
    public T Entity { get; }

    public EntityCreatedEvent(T entity, string type)
    {
        Entity = entity;
        Type = type;
    }
}