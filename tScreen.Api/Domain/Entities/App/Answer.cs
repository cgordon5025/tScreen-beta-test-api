using System;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Entities.Core;

namespace Domain.Entities.App
{
    [Table(nameof(Answer), Schema = Schema.TweenScreenApp)]
    public class Answer : BaseEntity
    {
        public Guid SessionId { get; set; }
        public Guid QuestionId { get; set; }
        public string? Data { get; set; }    
        public string? SentimentAnalysisData { get; set; }

        public Session? Session { get; set; }
        public Question? Question { get; set; }
    }
}