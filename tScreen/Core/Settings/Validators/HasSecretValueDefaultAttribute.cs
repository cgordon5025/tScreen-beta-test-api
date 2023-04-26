using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Settings.Validators;

[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)
]
public class HasSecretValueDefaultAttribute : ValidationAttribute
{
    public HasSecretValueDefaultAttribute()
        : base()
    {
        ErrorMessage = "Missing required application secret. Because this settings is marked as an application secret the value must be stored in a key vault";
    }

    protected override ValidationResult IsValid(object value, ValidationContext context)
    {
        var memberNames = new[] { context.MemberName };

        var propertyValue = (string)value;

        // Null and empty string are acceptable values. If the value is a required
        // then the require attribute should be used with addition to this validator
        if (string.IsNullOrWhiteSpace(propertyValue))
            return ValidationResult.Success;

        return propertyValue.Equals("<Application Secret Required>", StringComparison.InvariantCultureIgnoreCase)
            ? new ValidationResult(ErrorMessage, memberNames)
            : ValidationResult.Success;
    }
}