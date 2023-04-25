using System;
using System.Collections.Generic;
using HotChocolate.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    [Authorize]
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