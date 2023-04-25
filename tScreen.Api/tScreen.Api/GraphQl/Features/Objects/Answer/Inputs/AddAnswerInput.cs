using System;
using FluentValidation;
using GraphQl.GraphQl.Validators;

namespace GraphQl.GraphQl.Features.Objects.Answer.inputs;

public record AddAnswerInput(Guid SessionId, Guid QuestionId, string Data);

public class AddAnswerValidator : AbstractValidator<AddAnswerInput>
{
    public AddAnswerValidator()
    {
        RuleFor(e => e.SessionId)
            .MustBeNonEmptyGuid();

        RuleFor(e => e.QuestionId)
            .MustBeNonEmptyGuid();

        RuleFor(e => e.Data)
            .NotEmpty()
            .WithMessage("Required")
            .MustBeValidJson();
    }
}