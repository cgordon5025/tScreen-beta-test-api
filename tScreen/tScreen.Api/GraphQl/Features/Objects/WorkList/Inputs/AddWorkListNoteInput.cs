using System;
using FluentValidation;
using GraphQl.GraphQl.Validators;

namespace GraphQl.GraphQl.Features.Objects.WorkList.Inputs;

public record AddWorkListNoteInput(Guid WorkListId, NoteType Type, string Body, string? Data);

public enum NoteType
{
    Default
}

public class AddWorkListNoteValidator : AbstractValidator<AddWorkListNoteInput>
{
    public AddWorkListNoteValidator()
    {
        RuleFor(e => e.WorkListId).MustBeNonEmptyGuid();
        RuleFor(e => e.Type).IsInEnum();
        RuleFor(e => e.Body)
            .MaximumLength(3000)
            .WithMessage("Too long");

        When(e => e.Data is not null, () =>
        {
            RuleFor(e => e.Data!)
                .MustBeValidJson();
        });
    }
}
