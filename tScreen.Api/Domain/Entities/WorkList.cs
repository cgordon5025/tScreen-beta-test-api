using System;
using System.Collections.Generic;
using Domain.Common;
using Domain.Entities.App;

namespace Domain.Entities
{
    public class WorkList : BaseEntity
    {
        public Guid LocationId { get; set; }
        public Guid PersonId { get; set; }
        public Guid SessionId { get; set; }
        public string Type { get; set; }

        public Person? Person { get; set; } 
        public ICollection<WorkListNote> WorkListNotes { get; set; } = new HashSet<WorkListNote>();
        public ICollection<HistoryWorkList> HistoryWorkLists { get; set; } = new HashSet<HistoryWorkList>();
    }

    public static class WorkListStatus
    {
        public const string Unread = nameof(Unread);
        public const string Reviewed = nameof(Reviewed);
    }
}