using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Settings.Validators;

[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
    AllowMultiple = false)
]
public class HasDefaultValueAttribute : ValidationAttribute
{
    public HasDefaultValueAttribute()
        : base()
    {
        ErrorMessage = "Missing environment variable. Default value found, expect replacement.";
    }

    protected override ValidationResult IsValid(object value, ValidationContext context)
    {
        var memberNames = new [] { context.MemberName };

        if (value == null) return new ValidationResult(ErrorMessage, memberNames);

        var propertyValue = (string) value;
        return propertyValue.Equals("<Environment Variable Required>", StringComparison.InvariantCultureIgnoreCase) 
            ? new ValidationResult(ErrorMessage, memberNames) 
            : ValidationResult.Success;
    }
}