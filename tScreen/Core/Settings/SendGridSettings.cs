namespace Core.Settings;

public class SendGridSettings : IValidateSettings
{
    public string ApiKey { get; set; } = null;
}