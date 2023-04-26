using System;
using Domain.Common;
using Domain.Entities.App;

namespace Domain.Entities;

public class HistorySession : BaseEntity
{
    public Guid HistoryId { get; set; }
    public Guid SessionId { get; set; }

    public History? History { get; set; }
    public Session? Session { get; set; }
}