using System;
using System.Collections.Generic;
using HotChocolate.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    [Authorize]
    public class WorkList : BaseEntity
    {
        public Guid LocationId { get; set; }
        public Guid PersonId { get; set; }
        public Guid SessionId { get; set; }
        public string Type { get; set; } = null!;

        public Person? Person { get; set; }
        public ICollection<WorkListNote> WorkListNotes { get; set; } = new HashSet<WorkListNote>();
        public ICollection<HistoryWorkList> HistoryWorkLists { get; set; } = new HashSet<HistoryWorkList>();
    }
}