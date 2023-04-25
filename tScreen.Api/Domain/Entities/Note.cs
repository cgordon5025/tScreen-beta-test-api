using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities
{
    public class Note : BaseEntity
    {
        public string? Type { get; set; }
        public string? Body { get; set; }
        public string? Data { get; set; }

        public ICollection<WorkListNote> WorkListNotes { get; set; } = new HashSet<WorkListNote>();
        public ICollection<SessionNote> SessionNotes { get; set; } = new HashSet<SessionNote>();
    }

    public class NoteType
    {
        public const string Default = nameof(Default);
    }
}