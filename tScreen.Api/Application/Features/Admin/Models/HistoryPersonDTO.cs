using System;
using Application.Common.Models;

namespace Application.Features.Admin.Models;

public class HistoryPersonDTO : BaseEntityDTO
{
    public Guid HistoryId { get; set; }
    public Guid PersonId { get; set; }
        
    public HistoryDTO? History { get; set; }
    public PersonDTO? Person { get; set; }
}