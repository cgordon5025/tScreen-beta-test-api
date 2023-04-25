using System;
using Domain.Configurations;
using FluentValidation;
using GraphQl.GraphQl.Validators;

// ReSharper disable ClassNeverInstantiated.Global

namespace GraphQl.GraphQl.Features.Objects.CustomField.Inputs
{
    public record AddCustomFieldInput(
        Guid LocationId, CustomFieldType Type, string Name, string Description, 
        string Placeholder, int Position, string? DefaultValue, string? ValidationRule);

    public enum CustomFieldType
    {
        Text
    }
    
    public class AddCustomFieldValidator : AbstractValidator<AddCustomFieldInput>
    {
        public AddCustomFieldValidator()
        {
            RuleFor(e => e.LocationId)
                .MustBeNonEmptyGuid();

            RuleFor(e => e.Type)
                .IsInEnum();

            RuleFor(e => e.Name)
                .NotEmpty()
                .WithMessage("Required")
                .MaximumLength(FieldDefaults.StandardStringSize)
                .WithMessage("Too long");

            RuleFor(e => e.Description)
                .NotEmpty()
                .WithMessage("Required")
                .MaximumLength(FieldDefaults.StandardStringSize)
                .WithMessage("Too long");
            
            RuleFor(e => e.Placeholder)
                .NotEmpty()
                .WithMessage("Required")
                .MaximumLength(FieldDefaults.StandardStringSize)
                .WithMessage("Too long");

            RuleFor(e => e.Position)
                .GreaterThan(-1)
                .WithMessage("Only positive numbers allowed");

            RuleFor(e => e.DefaultValue)
                .MaximumLength(FieldDefaults.StandardStringSize)
                .WithMessage("Too long");
            
            RuleFor(e => e.DefaultValue)
                .MaximumLength(3000)
                .WithMessage("Too long");
        }
    }
}