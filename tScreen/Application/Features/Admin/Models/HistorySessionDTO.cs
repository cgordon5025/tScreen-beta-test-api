using System;
using Application.Common.Models;

namespace Application.Features.Admin.Models;

public class HistorySessionDTO : BaseEntityDTO
{
    public Guid HistoryId { get; set; }
    public Guid SessionId { get; set; }

    public HistoryDTO? History { get; set; }
    public SessionDTO? Session { get; set; }
}