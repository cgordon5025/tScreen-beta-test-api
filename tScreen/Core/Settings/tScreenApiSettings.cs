using System.ComponentModel.DataAnnotations;

namespace Core.Settings;

public class tScreenApiSettings : IValidateSettings
{
    public const string SectionName = "tScreenApi";

    [Required]
    public string BaseUrl { get; set; }
}