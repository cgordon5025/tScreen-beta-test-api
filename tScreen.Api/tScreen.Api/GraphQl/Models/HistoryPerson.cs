using System;
using HotChocolate.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    [Authorize]
    public class HistoryPerson : BaseEntity
    {
        public Guid HistoryId { get; set; }
        public Guid PersonId { get; set; }
        
        public History? History { get; set; }
        public Person? Person { get; set; }
    }
}