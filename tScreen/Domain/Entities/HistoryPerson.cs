using System;
using Domain.Common;

namespace Domain.Entities
{
    public class HistoryPerson : BaseEntity
    {
        public Guid HistoryId { get; set; }
        public Guid PersonId { get; set; }

        public History? History { get; set; }
        public Person? Person { get; set; }
    }
}