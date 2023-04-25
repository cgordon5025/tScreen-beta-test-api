using System;
using HotChocolate.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    [Authorize]
    public class HistoryWorkList : BaseEntity
    {
        public Guid HistoryId { get; set; }
        public Guid WorkListId { get; set; }
        
        public History? History { get; set; }
        public WorkList? WorkList { get; set; }
    }
}