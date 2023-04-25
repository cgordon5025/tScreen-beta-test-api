using System;

namespace Application.Common.Exceptions;

public class EntityIncompleteException : Exception
{
    public string TableName { get; }
    public Guid Id { get; }
    public string CurrentStatus { get; }
    public string ExpectedStatus { get; }

    public EntityIncompleteException(string tableName, Guid id, string currentStatus, string expectedStatus) 
        : base($"Entity operation cannot be performed on table {tableName}, record {id} because record has a status " +
               $"of {currentStatus} instead of {expectedStatus}.")
    {
        TableName = tableName;
        Id = id;
        CurrentStatus = currentStatus;
        ExpectedStatus = expectedStatus;
    }
}