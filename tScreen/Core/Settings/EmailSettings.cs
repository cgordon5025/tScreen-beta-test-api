using System.ComponentModel.DataAnnotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Core.Settings;

public class EmailSettings : IValidateSettings
{
    [Required] public string FromEmail { get; set; } = null!;
    [Required] public string FromName { get; set; } = null!;

    public EmailSettingsSmtp Smtp { get; set; }
}

public class EmailSettingsSmtp
{
    public string Hostname { get; set; }
    public int Port { get; set; }
}