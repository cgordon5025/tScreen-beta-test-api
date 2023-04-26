using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    [Authorize]

    public class History : BaseEntity
    {
        public Guid LocationId { get; set; }
        public Guid PersonId { get; set; }
        public string? Type { get; set; }
        public string? Data { get; set; }

        public ICollection<HistoryWorkList> HistoryWorkLists { get; set; } = new HashSet<HistoryWorkList>();
        public ICollection<HistoryPerson> HistoryPersons { get; set; } = new HashSet<HistoryPerson>();
        public ICollection<HistoryStudent> HistoryStudents { get; set; } = new HashSet<HistoryStudent>();
    }
}