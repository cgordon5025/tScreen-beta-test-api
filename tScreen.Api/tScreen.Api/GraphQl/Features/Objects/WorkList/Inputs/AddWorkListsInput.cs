using System;
using FluentValidation;
using GraphQl.GraphQl.Validators;

namespace GraphQl.GraphQl.Features.Objects.WorkList.Inputs;

// ReSharper disable once ClassNeverInstantiated.Global
public record AddWorkListsInput(Guid SessionId);

public class AddWorkListValidator : AbstractValidator<AddWorkListsInput>
{
    public AddWorkListValidator()
    {
        RuleFor(e => e.SessionId)
            .MustBeNonEmptyGuid();
    }
}