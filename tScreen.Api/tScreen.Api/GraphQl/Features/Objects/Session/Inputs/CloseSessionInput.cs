using System;
using FluentValidation;
using GraphQl.GraphQl.Validators;

namespace GraphQl.GraphQl.Features.Objects.Session.Inputs;

// ReSharper disable once ClassNeverInstantiated.Global
public record CloseSessionInput(Guid Id);


public class CloseSessionInputValidator : AbstractValidator<CloseSessionInput>
{
    public CloseSessionInputValidator()
    {
        RuleFor(e => e.Id)
            .MustBeNonEmptyGuid();
    }
}