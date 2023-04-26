using System;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    [Authorize]
    public class SceneQuestion : BaseEntity
    {
        public Guid SceneId { get; set; }
        public Guid QuestionId { get; set; }

        public Scene? Scene { get; set; }
        public Question? Question { get; set; }
    }
}