using System;
using System.Collections.Generic;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    [Authorize]
    public class Scene : BaseEntity
    {
        public Guid AdventureId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Position { get; set; }

        public Adventure? Adventure { get; set; } = default;

        public ICollection<SceneQuestion> SceneQuestions { get; set; } = new HashSet<SceneQuestion>();
    }
}