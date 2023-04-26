using System;

namespace Application.Common.Exceptions;

public class EntityDuplicateException : Exception
{
    public string TableName { get; }
    public object Columns { get; }
    public DateTime CreatedAt { get; }

    public EntityDuplicateException(string tableName, object columns, DateTime createdAt)
        : base($"Duplicate record with value {columns} created at {createdAt.ToString("O")} " +
                $"found in table {tableName}")
    {
        TableName = tableName;
        Columns = columns;
        CreatedAt = createdAt;
    }
}