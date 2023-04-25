using System.Collections.Generic;

namespace Application.Features.App.Answer.Models;

/// <summary>
/// The answer that maps to question
/// <see cref="MappedQuestionIds.WhatAboutSiblingsQuestionId"/>
/// </summary>
// ReSharper disable once MemberCanBePrivate.Global
// ReSharper disable once ClassNeverInstantiated.Global
public class AnswerDataSiblingsDTO
{
    public string? Name { get; set; }
    public IReadOnlyList<string>? Characteristics { get; set; } = null;
    public string? School { get; set; }
    public string? GradeLevel { get; set; }
}