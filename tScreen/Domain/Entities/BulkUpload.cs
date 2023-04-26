using System;
using Domain.Common;

namespace Domain.Entities;

public class BulkUpload : BaseEntity
{
    public Guid FileId { get; set; }
    public string Type { get; set; } = null!;
}

public static class BulkFileTypes
{
    public const string Person = nameof(Person);
    public const string Player = nameof(Player);
}