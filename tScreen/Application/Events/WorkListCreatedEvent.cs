using System;
using System.Collections.Generic;
using Application.Features.Admin.Models;

namespace Application.Events;

public class WorkListCreatedEvent : BaseEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public IEnumerable<WorkListDTO> Entities { get; }

    public WorkListCreatedEvent(IEnumerable<WorkListDTO> entities)
    {
        Entities = entities;
    }
}