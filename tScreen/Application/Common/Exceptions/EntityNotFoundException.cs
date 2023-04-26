using System;

namespace Application.Common.Exceptions;

public class EntityNotFoundException : Exception
{
    // ReSharper disable once MemberCanBePrivate.Global
    public string TableName { get; }
    public string Id { get; }

    public EntityNotFoundException(string tableName, Guid id)
        : base($"Entity not found in table {tableName} by id {id}")
    {
        TableName = tableName;
        Id = id.ToString();
    }

    public EntityNotFoundException(string tableName, string byField, Guid id)
        : base($"Entity not found in table {tableName} by {byField} {id}")
    {
        TableName = tableName;
        Id = id.ToString();
    }

    public EntityNotFoundException(string tableName, string id)
        : base($"Entity not found in table {tableName} by id {id}")
    {
        TableName = tableName;
        Id = id;
    }

    public EntityNotFoundException(string tableName, object id)
        : base($"Entity not found in table {tableName} by id {id}")
    {
        TableName = tableName;
        Id = id.ToString()!;
    }
}