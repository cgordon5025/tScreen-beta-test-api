using System.Collections.Generic;
using HotChocolate.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    [Authorize]
    public class Question : BaseEntity
    {
        public string? Category { get; set; }
        public string? Type { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public string? Data { get; set; }
        public int Position { get; set; }

        public ICollection<Answer> Answers { get; set; } = new HashSet<Answer>();
        public ICollection<SceneQuestion> SceneQuestions { get; set; } = new HashSet<SceneQuestion>();
        public ICollection<QuestionContingent> QuestionContingents { get; set; } = new HashSet<QuestionContingent>();
        public ICollection<QuestionContingent> QuestionParentContingents { get; set; } = new HashSet<QuestionContingent>();
    }
}