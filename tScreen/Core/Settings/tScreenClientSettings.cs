using System.ComponentModel.DataAnnotations;

namespace Core.Settings;

public class tScreenClientSettings : IValidateSettings
{
    public const string SectionName = "tScreenClient";

    [Required]
    public string BaseUrl { get; set; }
}