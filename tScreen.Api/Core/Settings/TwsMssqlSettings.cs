using System.ComponentModel.DataAnnotations;

namespace Core.Settings;

public class TwsMssqlSettings : IValidateSettings
{
    [Required]
    public string ConnectionString { get; set; }
}