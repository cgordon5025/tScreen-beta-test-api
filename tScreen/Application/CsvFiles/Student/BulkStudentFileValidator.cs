using System;
using System.Text.RegularExpressions;
using FluentValidation;

namespace Application.CsvFiles.Student;

public sealed class BulkStudentFileValidator : AbstractValidator<BulkStudentFile>
{
    public BulkStudentFileValidator()
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

        RuleFor(c => c.Dob)
            .NotEmpty()
            .WithMessage("Required");

        When(c => !string.IsNullOrWhiteSpace(c.ParentPhone), () =>
        {
            RuleFor(c => c.ParentPhone)
                .Must(BeValidPhoneNumber)
                .WithMessage("Invalid phone number");
        });

        When(c => !string.IsNullOrWhiteSpace(c.ParentEmail), () =>
        {
            RuleFor(c => c.ParentEmail)
                .EmailAddress()
                .WithMessage("Invalid email");
        });

        When(c => !string.IsNullOrWhiteSpace(c.PostalCode), () =>
        {
            RuleFor(c => c.PostalCode)
                .Must(postalCode =>
                {
                    var postalCodeClean = Regex.Replace(postalCode, "\\s+|\\-+", "");
                    return Regex.IsMatch(postalCodeClean, "^([0-9]{5})([0-9]{4})?");
                })
                .WithMessage("Zip code is invalid");
        });

        When(c => !string.IsNullOrWhiteSpace(c.ParentEmail), () =>
        {
            RuleFor(c => c.ParentEmail)
                .EmailAddress()
                .WithMessage("Invalid email");
        });

        // RuleFor(c => c.Grade)
        //     .NotEmpty()
        //     .WithMessage("Required");
    }

    private static bool BeValidPhoneNumber(string? phoneNumber)
    {
        if (phoneNumber is null) throw new ArgumentNullException(nameof(phoneNumber));

        // Regular expression based on the NAP (North American Numbering Plan)
        var phoneNumberUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
        var phoneNumberData = phoneNumberUtil.Parse(phoneNumber, "US");

        return phoneNumberUtil.IsValidNumberForRegion(phoneNumberData, "US");
    }
}