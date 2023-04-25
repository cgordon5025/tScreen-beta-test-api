using System.ComponentModel.DataAnnotations;

namespace Core.Settings;

public class TweenScreenApiSettings : IValidateSettings
{
    public const string SectionName = "TweenScreenApi";
    
    [Required]
    public string BaseUrl { get; set; }
}