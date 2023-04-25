using System.Collections.Generic;
using Application.Common.Models;

namespace Application.Features.Admin.Models;

public class NoteDTO : BaseEntityDTO
{
    public string? Type { get; set; }
    public string? Body { get; set; }
    public string? Data { get; set; }

    public ICollection<WorkListNoteDTO> WorkListNotes { get; set; } = new HashSet<WorkListNoteDTO>();
    public ICollection<SessionNoteDTO> SessionNotes { get; set; } = new HashSet<SessionNoteDTO>();
}