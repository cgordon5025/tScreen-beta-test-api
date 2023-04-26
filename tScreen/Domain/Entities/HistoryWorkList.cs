using System;
using Domain.Common;

namespace Domain.Entities
{
    public class HistoryWorkList : BaseEntity
    {
        public Guid HistoryId { get; set; }
        public Guid WorkListId { get; set; }

        public History? History { get; set; }
        public WorkList? WorkList { get; set; }
    }
}