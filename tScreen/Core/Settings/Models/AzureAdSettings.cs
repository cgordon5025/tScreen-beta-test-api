using System.ComponentModel.DataAnnotations;
using Core.Settings.Validators;

namespace Core.Settings.Models;

public class AzureAdSettings : IValidateSettings
{
    [Required, HasSecretValueDefault]
    public string TenantId { get; set; }

    [Required, HasSecretValueDefault]
    public string ResourceId { get; set; }

    [Required, HasSecretValueDefault]
    public string Instance { get; set; }

    public string Audience => ResourceId.Replace("api://", "");
    public string Authority => $"{Instance}/{TenantId}/v2.0";
}