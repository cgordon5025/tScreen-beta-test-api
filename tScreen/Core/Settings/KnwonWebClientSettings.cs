using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Settings;

public class KnownWebClientsSettings : IValidateSettings
{
    [Required]
    public IEnumerable<string> Domains { get; set; }
}