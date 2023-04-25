using System;

namespace Application.Common.Exceptions;

public class EntityCompleteException : Exception
{
    public DateTime? CompleteAt { get; }

    public EntityCompleteException(string message, DateTime? completeAt) : base(message)
    {
        CompleteAt = completeAt;
    }
}