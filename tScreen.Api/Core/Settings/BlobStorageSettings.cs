using System;
using System.ComponentModel.DataAnnotations;
using Core.Settings.Validators;

namespace Core.Settings;

public class BlobStorageSettings : IValidateSettings
{
    private const string AccountNameToken = "<ACCOUNT_NAME>";
    private const string AccountKeyToken = "<ACCOUNT_KEY>";
    
    
    public string AccountName { get; set; }
    
   
    public string AccountKey { get; set; }
    
    [Required]
    public string ConnectionString { get; set; }
    
    public string StorageUrl { get; set; }

    public string PreparedConnectionString {
        get
        {
            if (ConnectionString == null)
                throw new ArgumentNullException(nameof(ConnectionString));

            if (AccountName == null)
                throw new ArgumentNullException(nameof(AccountName));

            if (AccountKey == null)
                throw new ArgumentNullException(nameof(AccountKey));
            
            var connectionString = ConnectionString
                .Replace(AccountNameToken, AccountName)
                .Replace(AccountKeyToken, AccountKey);

            return connectionString;
        }
    }
}