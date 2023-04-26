using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    [Authorize]
    public class Note : BaseEntity
    {
        public Guid WorkListId { get; set; }
        public string? Type { get; set; }
        public string? Body { get; set; }
        public string? Data { get; set; }

        public ICollection<WorkListNote> WorkListNotes { get; set; } = new HashSet<WorkListNote>();
        public ICollection<SessionNote> SessionNotes { get; set; } = new HashSet<SessionNote>();
    }
}