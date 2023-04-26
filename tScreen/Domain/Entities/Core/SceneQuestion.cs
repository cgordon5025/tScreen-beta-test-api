using System;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;

namespace Domain.Entities.Core
{
    [Table(nameof(SceneQuestion), Schema = Schema.tScreenCore)]
    public class SceneQuestion : BaseEntity
    {
        public Guid SceneId { get; set; }
        public Guid QuestionId { get; set; }

        public Scene? Scene { get; set; }
        public Question? Question { get; set; }
    }
}