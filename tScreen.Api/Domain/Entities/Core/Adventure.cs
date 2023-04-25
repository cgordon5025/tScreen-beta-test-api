using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Entities.App;

namespace Domain.Entities.Core
{
    [Table(nameof(Adventure), Schema = Schema.TweenScreenCore)]
    public class Adventure : BaseEntity
    {
        public Guid FileId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Position { get; set; }
        
        public CoreFile? CoreFile { get; set; }
        
        public ICollection<Session> Sessions { get; set; } = new HashSet<Session>();
        public ICollection<Scene> Scenes { get; set; } = new HashSet<Scene>();
    }
}