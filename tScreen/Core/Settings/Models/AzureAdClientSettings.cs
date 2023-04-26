using System.ComponentModel.DataAnnotations;
using Core.Settings.Validators;

namespace Core.Settings.Models;

public class AzureAdClientSettings : IValidateSettings
{
    [Required, HasSecretValueDefault]
    public string TenantId { get; set; }

    [Required, HasSecretValueDefault]
    public string ResourceId { get; set; }

    [Required, HasSecretValueDefault]
    public string InstanceId { get; set; }

    [Required, HasSecretValueDefault]
    public string ClientId { get; set; }

    [HasSecretValueDefault]
    public string ClientSecret { get; set; }

    [HasSecretValueDefault]
    public string CertificateName { get; set; }

    public string Authority => $"{InstanceId}/{TenantId}/v2.0";
}