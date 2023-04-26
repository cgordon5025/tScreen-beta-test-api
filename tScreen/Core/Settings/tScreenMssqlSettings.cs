using System.ComponentModel.DataAnnotations;

namespace Core.Settings;

public class tScreenMssqlSettings : IValidateSettings
{
    [Required]
    public string ConnectionString { get; set; }
}