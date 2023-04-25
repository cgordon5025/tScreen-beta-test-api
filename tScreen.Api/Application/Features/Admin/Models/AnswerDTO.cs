using System;
using Application.Common.Models;

namespace Application.Features.Admin.Models;

public class AnswerDTO : BaseEntityDTO
{
    public Guid SessionId { get; set; }
    public Guid QuestionId { get; set; }
    public string? Data { get; set; }    
    public string? SentimentAnalysisData { get; set; }

    public SessionDTO? Session { get; set; }
    public QuestionDTO? Question { get; set; }
}