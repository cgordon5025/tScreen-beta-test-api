using System;

namespace Application.Common.Exceptions;

public class EntityIdRequiredException : Exception
{
    public EntityIdRequiredException(string paramName)
        : base($"{paramName} required. Empty GUID not allowed")
    {

    }
}