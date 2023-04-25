using System;
using HotChocolate.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    [Authorize]
    public class SessionNote : BaseEntity
    {
        public Guid NoteId { get; set; }
        public Guid SessionId { get; set; }
        
        public Note? Note { get; set; }
        public Session? Session { get; set; }
    }
}