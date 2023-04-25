using System.ComponentModel.DataAnnotations;

namespace Core.Settings;

public class KeyVaultSetting : IValidateSettings
{
    // Support for azure and other environments which dont support managed identities
    // E.g., some services in preview like azure multi-container app services at the time
    // of composing this comment.
    public string TenantId { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    
    public string UserAssignedId { get; set; }
    
    [Required]
    public string VaultUri { get; set; }
}