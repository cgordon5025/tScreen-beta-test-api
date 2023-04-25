using System;
using Core;
using FluentValidation;

// ReSharper disable UnusedMethodReturnValue.Global

namespace GraphQl.GraphQl.Validators;

public static class FluentValidatorsExtensions
{
    public static IRuleBuilder<T, Guid> MustBeNonEmptyGuid<T>(this IRuleBuilder<T, Guid> builder)
        => builder
            .Must((guid) => guid != Guid.Empty)
            .WithMessage("Valid GUID required. Cannot use Empty GUID");
    
    public static IRuleBuilder<T, Guid?> MustBeNonEmptyGuid<T>(this IRuleBuilder<T, Guid?> builder)
        => builder
            .Must((guid) => guid != Guid.Empty)
            .WithMessage("Valid GUID required. Cannot use Empty GUID");
    
    public static IRuleBuilder<T, string> MustBeValidJson<T>(this IRuleBuilder<T, string> builder)
        => builder
            .Must(Utility.IsValidJson)
            .WithMessage("Valid JSON required");
}