using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;

namespace Domain.Entities.Core
{
    [Table(nameof(Scene), Schema = Schema.tScreenCore)]
    public class Scene : BaseEntity
    {
        public Guid AdventureId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Position { get; set; }

        public Adventure? Adventure { get; set; }

        public ICollection<SceneQuestion> SceneQuestions { get; set; } = new HashSet<SceneQuestion>();
    }
}