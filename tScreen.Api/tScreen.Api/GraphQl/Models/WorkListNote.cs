using System;
using GraphQl.GraphQl.Interfaces;
using HotChocolate.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    [Authorize]
    public class WorkListNote : BaseEntity, IWorkListResult
    {
        public Guid WorkListId { get; set; }
        public Guid NoteId { get; set; }
        public WorkList? WorkList { get; set; }
        public Note? Note { get; set; }
    }
}