using System;
using Domain.Common;
using Domain.Entities.App;

namespace Domain.Entities
{
    public class SessionNote : BaseEntity
    {
        public Guid NoteId { get; set; }
        public Guid SessionId { get; set; }

        public Note? Note { get; set; }
        public Session? Session { get; set; }
    }
}