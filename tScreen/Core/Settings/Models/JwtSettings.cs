using System.ComponentModel.DataAnnotations;
using Core.Settings.Validators;

namespace Core.Settings.Models;

public class JwtSettings : IValidateSettings
{
    [Required, HasDefaultValue]
    public string Authority { get; set; }

    [Required, HasDefaultValue]
    public string Audience { get; set; }

    [Required]
    public int ExpiryInMinutes { get; set; }

    [Required, HasDefaultValue]
    public string SigningKey { get; set; }

    public string StudentAudience { get; set; }
}