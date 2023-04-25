using System;
using Domain.Common;
using Domain.Entities.App;

namespace Domain.Entities;

public class ReportVersion : BaseEntity
{
    public Guid SessionId { get; set; }
    public string ClassName { get; set; } = null!;
    public string MethodName { get; set; } = null!;
    public string Version { get; set; } = null!;

    public Session? Session { get; set; }
}