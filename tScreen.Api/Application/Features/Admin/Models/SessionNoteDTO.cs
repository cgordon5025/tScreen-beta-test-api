using System;
using Application.Common.Models;

namespace Application.Features.Admin.Models;

public class SessionNoteDTO : BaseEntityDTO
{
    public Guid NoteId { get; set; }
    public Guid SessionId { get; set; }
        
    public NoteDTO? Note { get; set; }
    public SessionDTO? Session { get; set; }
}