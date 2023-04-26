using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Entities.App;

namespace Domain.Entities.Core
{
    [Table(nameof(Question), Schema = Schema.tScreenCore)]
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

    public static class QuestionCategories
    {
        public const string Environment = nameof(Environment);
        public const string Personal = nameof(Personal);
        public const string Survey = nameof(Survey);
    }

    public static class QuestionTypes
    {
        public const string Scale = nameof(Scale);
        public const string Boolean = nameof(Boolean);
        public const string Freeform = nameof(Freeform);
        public const string Composite = nameof(Composite);
        public const string Multiselect = nameof(Multiselect);
    }
}