using System;
using System.Collections.Generic;
using Application.Common.Models;

namespace Application.Features.Admin.Models;

public class WorkListDTO : BaseEntityDTO
{
    public Guid LocationId { get; set; }
    public Guid PersonId { get; set; }
    public Guid SessionId { get; set; }
    public string Type { get; set; }
    
    public PersonDTO? Person { get; set; } 
    public ICollection<WorkListNoteDTO> WorkListNotes { get; set; } = new HashSet<WorkListNoteDTO>();
    public ICollection<HistoryWorkListDTO> HistoryWorkLists { get; set; } = new HashSet<HistoryWorkListDTO>();
}