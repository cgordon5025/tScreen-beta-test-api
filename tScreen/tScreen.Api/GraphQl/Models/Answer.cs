using System;
using GraphQl.GraphQl.Interfaces;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    [Authorize]
    public class Answer : BaseEntity, IAnswerResult
    {
        public Guid SessionId { get; set; }
        public Guid QuestionId { get; set; }
        public string? Data { get; set; }
        public string? SentimentAnalysisData { get; set; }

        public Session? Session { get; set; }
        public Question? Question { get; set; }
    }
}