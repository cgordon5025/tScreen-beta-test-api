using System;
using Domain.Common;

namespace Domain.Entities
{
    public class WorkListNote : BaseEntity
    {
        public Guid WorkListId { get; set; }
        public Guid NoteId { get; set; }

        public WorkList? WorkList { get; set; }
        public Note? Note { get; set; }
    }
}