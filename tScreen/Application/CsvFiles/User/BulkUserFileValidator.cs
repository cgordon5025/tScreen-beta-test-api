using FluentValidation;

namespace Application.CsvFiles.User;

public sealed class BulkUserFileValidator : AbstractValidator<BulkUserFile>
{
    public BulkUserFileValidator()
    {
        RuleFor(c => c.FirstName)
            .NotEmpty()
            .WithMessage("Required")
            .MaximumLength(255)
            .WithMessage("Too long");

        RuleFor(c => c.MiddleName)
            .MaximumLength(255)
            .WithMessage("Too long");

        RuleFor(c => c.LastName)
            .NotEmpty()
            .WithMessage("Required")
            .MaximumLength(255)
            .WithMessage("Too long");

        RuleFor(c => c.JobTitle)
            .MaximumLength(255)
            .WithMessage("Too long");

        RuleFor(c => c.UserRole)
            .NotEmpty()
            .WithMessage("Required");

        RuleFor(c => c.Email)
            .NotEmpty()
            .WithMessage("Required")
            .EmailAddress()
            .WithMessage("Invalid email");

        RuleFor(c => c.StartDate)
            .NotEmpty()
            .WithMessage("Required");
    }
}