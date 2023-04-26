using System;
using Domain.Common;

namespace Domain.Entities
{
    public class HistoryStudent : BaseEntity
    {
        public Guid HistoryId { get; set; }
        public Guid StudentId { get; set; }

        public History? History { get; set; }
        public Student? Student { get; set; }
    }
}