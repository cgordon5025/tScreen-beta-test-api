using System.Collections.Generic;

namespace Application.Features.App.Answer.Models;

/// <summary>
/// The answer that maps to question
/// <see cref="MappedQuestionIds.ParentDescribeTheirMoodQuestionId"/>
/// </summary>
// ReSharper disable once UnusedType.Global
public class AnswerDataParentMood
{
    public string? Name { get; set; }
    public IList<string> Characteristics { get; set; } = new List<string>();
}