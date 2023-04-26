using System;
using FluentValidation;
using GraphQl.GraphQl.Validators;

namespace GraphQl.GraphQl.Features.Objects.Session.Inputs;

public record EditSessionInput(Guid SessionId, Guid AdventureId, Guid AvatarId);

public class EditSessionInputValidator : AbstractValidator<EditSessionInput>
{
    public EditSessionInputValidator()
    {
        RuleFor(e => e.SessionId)
            .MustBeNonEmptyGuid();

        RuleFor(e => e.AdventureId)
            .MustBeNonEmptyGuid();

        RuleFor(e => e.AvatarId)
            .MustBeNonEmptyGuid();
    }
}