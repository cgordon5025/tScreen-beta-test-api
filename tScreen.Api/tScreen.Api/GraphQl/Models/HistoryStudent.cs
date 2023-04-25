using System;
using HotChocolate.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    [Authorize]
    public class HistoryStudent : BaseEntity
    {
        public Guid HistoryId { get; set; }
        public Guid StudentId { get; set; }
        
        public History? History { get; set; }
        public Student? Student { get; set; }
    }
}