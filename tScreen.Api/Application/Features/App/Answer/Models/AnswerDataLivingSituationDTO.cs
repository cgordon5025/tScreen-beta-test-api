using System.Collections.Generic;

namespace Application.Features.App.Answer.Models;

/// <summary>
/// The answer that maps to question
/// <see cref="MappedQuestionIds.WhoDoYouLiveWithQuestionId"/>
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public class AnswerDataLivingSituationDTO
{
    public string GuardianType { get; init; } = null!;
    public string LivingSituation { get; init; } = null!;
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public IReadOnlyList<string> SelectedParents { get; init; } = new List<string>();
    public bool GrandParentsLiveAtResidence { get; init; } = false;
    public string? OtherRelationship { get; init; }
}

public class GuardianType
{
    public const string Parent = nameof(Parent);
    public const string TrustedAdult = nameof(TrustedAdult);
}

public class LivingSituation
{
    public const string LivesWithOneParent = nameof(LivesWithOneParent);
    public const string LivesWithBothParents = nameof(LivesWithBothParents);
    public const string SplitTimeEvenly = nameof(SplitTimeEvenly);
    public const string SplitTimeMainlyLiveWith = nameof(SplitTimeMainlyLiveWith);

    public const string LivesWithGrandparents = nameof(LivesWithGrandparents);
    public const string SiblingsAreCareTakers = nameof(SiblingsAreCareTakers);
    public const string LivesWithSomeoneOtherThanFamily = nameof(LivesWithSomeoneOtherThanFamily);
}