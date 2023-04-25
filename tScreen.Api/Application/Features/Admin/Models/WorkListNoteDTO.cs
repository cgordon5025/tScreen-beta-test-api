using System;
using Application.Common.Models;

namespace Application.Features.Admin.Models;

public class WorkListNoteDTO : BaseEntityDTO
{
    public Guid WorkListId { get; set; }
    public Guid NoteId { get; set; }
        
    public WorkListDTO? WorkList { get; set; }
    public NoteDTO? Note { get; set; }
}