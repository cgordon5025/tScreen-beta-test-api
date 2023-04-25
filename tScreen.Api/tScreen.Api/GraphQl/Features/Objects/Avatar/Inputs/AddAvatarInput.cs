using System;
using System.Drawing;
using FluentValidation;
using GraphQl.GraphQl.Validators;

namespace GraphQl.GraphQl.Features.Objects.Avatar.Inputs;

// ReSharper disable once ClassNeverInstantiated.Global
public record AddAvatarInput(Guid StudentId, Guid? BodyId, Guid? HelperId,
    string? BodyColor, string? EyeColor, string? HairColor, string? ShirtColor, 
    string? PantsColor, string? ShoesColor);

public class AddAvatarValidator : AbstractValidator<AddAvatarInput>
{
    public AddAvatarValidator()
    {
        RuleFor(e => e.StudentId)
            .MustBeNonEmptyGuid();
        
        When(e => e.BodyId is not null, () =>
        {
            RuleFor(e => e.BodyId)
                .MustBeNonEmptyGuid();
        });

        When(e => e.HelperId is not null, () =>
        {
            RuleFor(e => e.HelperId)
                .MustBeNonEmptyGuid();
        });

        When(e => e.BodyColor is not null, () =>
        {
            RuleFor(e => e.BodyColor)
                .Must(BeHtmlColor!)
                .WithMessage("Color code provided invalid");
        });
        
        When(e => e.EyeColor is not null, () =>
        {
            RuleFor(e => e.EyeColor)
                .Must(BeHtmlColor!)
                .WithMessage("Color code provided invalid");
        });
        
        When(e => e.HairColor is not null, () =>
        {
            RuleFor(e => e.HairColor)
                .Must(BeHtmlColor!)
                .WithMessage("Color code provided invalid");
        });
        
        When(e => e.ShirtColor is not null, () =>
        {
            RuleFor(e => e.ShirtColor)
                .Must(BeHtmlColor!)
                .WithMessage("Color code provided invalid");
        });
        
        When(e => e.PantsColor is not null, () =>
        {
            RuleFor(e => e.PantsColor)
                .Must(BeHtmlColor!)
                .WithMessage("Color code provided invalid");
        });
        
        When(e => e.ShoesColor is not null, () =>
        {
            RuleFor(e => e.ShoesColor)
                .Must(BeHtmlColor!)
                .WithMessage("Color code provided invalid");
        });
    }

    private static bool BeHtmlColor(string htmlColor)
    {
        try
        {
            var _ = ColorTranslator.FromHtml(htmlColor);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}