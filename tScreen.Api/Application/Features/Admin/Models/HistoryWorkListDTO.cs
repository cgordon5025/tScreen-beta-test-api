using System;
using Application.Common.Models;

namespace Application.Features.Admin.Models;

public class HistoryWorkListDTO : BaseEntityDTO
{
    public Guid HistoryId { get; set; }
    public Guid WorkListId { get; set; }
        
    public HistoryDTO? History { get; set; }
    public WorkListDTO? WorkList { get; set; }
}