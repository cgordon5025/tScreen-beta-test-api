using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities.App;
using FluentValidation;
using GraphQl.GraphQl.Validators;

namespace GraphQl.GraphQl.Features.Objects.Session.Inputs;

// ReSharper disable once ClassNeverInstantiated.Global
public record ShiftSessionCheckpointInput(Guid SessionId,
    Guid? AdventureId,
    Guid? AvatarId,
    CheckpointType Type,
    List<SessionAnswerInput>? Answers,
    bool? UsePreviousSession = false);

public record SessionAnswerInput(Guid QuestionId, string Data);

public enum CheckpointType
{
    AuthenticationSuccess,
    AvatarSelected,
    EnvironmentQuestions,
    PersonalQuestions,
    AdventureSelected,
    AdventureComplete,
    AppClosed
}

public class ShiftSessionCheckpointValidator : AbstractValidator<ShiftSessionCheckpointInput>
{
    private readonly CheckpointType[] _answersRequiredForCheckpointTypes = {
        CheckpointType.EnvironmentQuestions,
        CheckpointType.PersonalQuestions,
        // CheckpointType.AdventureSelected,
        CheckpointType.AdventureComplete
    };

    public ShiftSessionCheckpointValidator()
    {
        RuleFor(e => e.SessionId)
            .MustBeNonEmptyGuid();

        When(e => e.UsePreviousSession == true, () =>
        {
            RuleFor(e => e.UsePreviousSession)
                .Must((context, _) =>
                    context.Type == CheckpointType.EnvironmentQuestions)
                .WithMessage(i => "Previous session can only be defined at checkpoint " +
                                  $"\"{CheckpointType.EnvironmentQuestions}.\" Selected checkpoint \"{i.Type}\" is invalid.");
        });

        When(e => e.Answers is null ||
                  (e.Type != CheckpointType.EnvironmentQuestions && !e.Answers.Any()), () =>
                  {
                      RuleFor(e => e.Answers)
                          .Must((context, _)
                              => !_answersRequiredForCheckpointTypes.Contains(context.Type))
                          .WithMessage(i => $"Answers must be provided when shifting to checkpoint type {i.Type}");
                  });

        When(e => e.AdventureId is not null, () =>
        {
            RuleFor(e => e.AdventureId)
                .MustBeNonEmptyGuid()
                .Must((context, _)
                    => context.Type.ToString() == SessionCheckpoints.AdventureSelected)
                .WithMessage($"Can only change AdventureId when state is {nameof(SessionCheckpoints.AdventureSelected)}");

            RuleFor(e => e.Type)
                .Must(e => e.ToString() == SessionCheckpoints.AdventureSelected)
                .WithMessage($"Type must be {nameof(SessionCheckpoints.AdventureSelected)} when specifying an AdventureId");
        });

        When(e => e.AvatarId is not null, () =>
        {
            RuleFor(e => e.AvatarId)
                .Must((context, _)
                    => context.Type.ToString() == SessionCheckpoints.AvatarSelected)
                .WithMessage($"Can only change AvatarId when state is {nameof(SessionCheckpoints.AvatarSelected)}");

            RuleFor(e => e.Type)
                .Must(e => e.ToString() == SessionCheckpoints.AvatarSelected)
                .WithMessage($"Type must be {nameof(SessionCheckpoints.AvatarSelected)} when specifying an AdventureId");
        });

        RuleFor(e => e.Type)
            .IsInEnum();

        When(e => e.Type.ToString() == SessionCheckpoints.AvatarSelected, () =>
        {
            RuleFor(e => e.AvatarId)
                .MustBeNonEmptyGuid()
                .Must(e => e is not null)
                .WithMessage("AvatarId must be provided when shifting to checkpoint " +
                             nameof(SessionCheckpoints.AvatarSelected));
        });

        When(e => e.Type.ToString() == SessionCheckpoints.AdventureSelected, () =>
        {
            RuleFor(e => e.AdventureId)
                .MustBeNonEmptyGuid()
                .Must(e => e is not null)
                .WithMessage("AdventureId must be provided when shifting to checkpoint " +
                             nameof(SessionCheckpoints.AdventureSelected));
        });
    }
}