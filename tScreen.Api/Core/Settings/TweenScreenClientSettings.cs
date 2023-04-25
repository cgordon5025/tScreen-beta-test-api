using System.ComponentModel.DataAnnotations;

namespace Core.Settings;

public class TweenScreenClientSettings : IValidateSettings
{
    public const string SectionName = "TweenScreenClient";

    [Required]
    public string BaseUrl { get; set; }
}