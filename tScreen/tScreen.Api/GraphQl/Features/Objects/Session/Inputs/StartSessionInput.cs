using System;
using FluentValidation;
using GraphQl.GraphQl.Validators;

namespace GraphQl.GraphQl.Features.Objects.Session.Inputs;


// ReSharper disable once ClassNeverInstantiated.Global
public record StartSessionInput(Guid StudentId, SessionType Type);

public enum SessionType
{
    Full,
    Partial
}

public class StartSessionValidator : AbstractValidator<StartSessionInput>
{
    public StartSessionValidator()
    {
        RuleFor(e => e.StudentId)
            .MustBeNonEmptyGuid();

        RuleFor(e => e.Type)
            .IsInEnum();
    }
}