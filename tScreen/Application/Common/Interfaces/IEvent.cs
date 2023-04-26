using System;

namespace Application.Common.Interfaces;

public interface IEvent
{
    public Guid Id { get; }
    public string Type { get; }
}