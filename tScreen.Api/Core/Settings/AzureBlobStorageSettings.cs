#nullable enable
using System;
using System.Text;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Core.Settings;

// ReSharper disable once ClassNeverInstantiated.Global
public class AzureBlobStorageSettings : IValidateSettings
{
    private readonly ILogger<AzureBlobStorageSettings> _logger;

    private enum Types
    {
        Blob,
        Queue,
        Table,
        File
    }
    
    public const string AzuriteDnsName = "azurite";
    

    // ReSharper disable once MemberCanBePrivate.Global
    public const string DevelopmentConnectionString = "UseDevelopmentStorage=true";
    
    private const int StoragePort = 10000;
    private const int QueuePort = 10001;
    private const int TablePort = 10002;
    private const int FilePort = 10003;
    
    private const string LocalEndpointProtocol = "http";
    private const string AzureEndpointProtocol = "https";
    private const string AzuriteDefaultAccountName = "devstoreaccount1";
    private const string AzuriteDefaultAccountKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";

    public AzureBlobStorageSettings()
    {
        _logger = NullLogger<AzureBlobStorageSettings>.Instance;
#if DEBUG
    using var loggerFactory = LoggerFactory.Create(builder => builder
        .SetMinimumLevel(LogLevel.Debug)
        .AddConsole());
    _logger = loggerFactory.CreateLogger<AzureBlobStorageSettings>();
#endif
    }
    
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string? DnsName { get; set; } = null!;
    
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public string? AccountName { get; set; } = null!;
    
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public string? AccountKey { get; set; } = null!;

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string? UserAssignedId { get; set; } = null!;
    
    // Support for azure and other environments which dont support managed identities
    // E.g., some services in preview like azure multi-container app services at the time
    // of composing this comment.
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public string? TenantId { get; set; } = null!;
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public string? ClientId { get; set; } = null!;
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public string? ClientSecret { get; set; } = null!;

    public Uri GetStorageEndpointUri(string containerName, string? dnsName = null)
    {
#if DEBUG
using (_logger.BeginScope(nameof(GetStorageEndpointUri)))
    _logger.LogDebug("Using {Method} with parameters containerName: {ContainerName}, dsnName: {DsnName}", 
        nameof(GetStorageEndpointUri), containerName, dnsName ?? "Null");    
#endif
        
        var connectionString = dnsName is not null
            ? $"{LocalEndpointProtocol}://{dnsName}:{StoragePort}/{GetAccountName()}/{containerName}"
            : IsUsingDevelopmentStorage
                ? $"{LocalEndpointProtocol}://127.0.0.1:{StoragePort}/{GetAccountName()}/{containerName}"
                : $"{AzureEndpointProtocol}://{AccountName}.blob.core.windows.net/{containerName}";

#if DEBUG
using (_logger.BeginScope(nameof(GetStorageEndpointUri)))
{
    _logger.LogDebug("AccountName: {AccountName}, IsUsingDevelopmentStorage: {UsingDevelopmentStorage}", 
        AccountName, IsUsingDevelopmentStorage);
    _logger.LogDebug("Connection string: {ConnectionString}", connectionString);
}
#endif
        
        return new Uri(connectionString);
    }
    
    public string GetStorageConnectionString(bool partial = false)
    {
        var domain =  GetDnsName();
        var accountName = GetAccountName();
        var accountKey = GetAccountKey();
        
        var connectionStringBody = GetConnectionStringByType(Types.Blob, domain, accountName);

        if (partial) return connectionStringBody;

        var connectionStringHead = string.IsNullOrWhiteSpace(UserAssignedId)
            ? $"AccountName={accountName};AccountKey={accountKey};DefaultEndpointsProtocol={AzureEndpointProtocol};"
            : $"AccountName={accountName};DefaultEndpointsProtocol={LocalEndpointProtocol};";

        return $"{connectionStringHead}{connectionStringBody}";
    }

    /// <summary>
    /// Get Blob Storage Queue endpoint
    /// </summary>
    /// <remarks>
    /// Based on the type of hosting environment get return the the correct Queue Endpoint. The environments
    /// supported are, azurite (local/docker), and Azure Cloud
    /// </remarks>
    /// <param name="queueName">Queue name</param>
    /// <param name="dnsName">
    ///  Replace the default localhost IP address (127.0.0.1). Used with Docker environments
    /// </param>
    /// <returns>Fully qualified URI string</returns>
    public Uri GetQueueEndpointUri(string queueName, string? dnsName = null)
    {
        var connectionString = dnsName is not null
            ? $"{LocalEndpointProtocol}://{dnsName}:{QueuePort}/{GetAccountName()}/{queueName}"
            : IsUsingDevelopmentStorage
                ? $"{LocalEndpointProtocol}://127.0.0.1:{QueuePort}/{GetAccountName()}/{queueName}"
                : $"{AzureEndpointProtocol}://{AccountName}.queue.core.windows.net/{queueName}";

        #if DEBUG
        using (_logger.BeginScope(nameof(GetQueueEndpointUri)))
        {
            _logger.LogDebug("Query connection string: {ConnectionString}", connectionString);
        }
        #endif
        
        return new Uri(connectionString);
    }

    public string GetQueueConnectionString(bool partial = false)
    {
        var domain =  GetDnsName();
        var accountName = GetAccountName();
        var accountKey = GetAccountKey();
        
        var connectionStringBody = GetConnectionStringByType(Types.Queue, domain, accountName);

        if (partial) return connectionStringBody;

        var connectionStringHead = string.IsNullOrWhiteSpace(UserAssignedId)
            ? $"AccountName={accountName};AccountKey={accountKey};DefaultEndpointsProtocol={AzureEndpointProtocol};"
            : $"AccountName={accountName};DefaultEndpointsProtocol={LocalEndpointProtocol};";

        return $"{connectionStringHead}{connectionStringBody}";
    }

    public Uri GetTableEndpointUri(string tableName, string? dnsName = null)
    {
        var connectionString = dnsName is not null
            ? $"{LocalEndpointProtocol}://{dnsName}:{TablePort}/{GetAccountName()}/{tableName}"
            : IsUsingDevelopmentStorage
                ? $"{LocalEndpointProtocol}://127.0.0.1:{TablePort}/{GetAccountName()}/{tableName}"
                : $"{AzureEndpointProtocol}://{AccountName}.table.core.windows.net/{tableName}";

        return new Uri(connectionString);
    }
    
    public string GetTableConnectionString(bool partial = false)
    {
        var domain =  GetDnsName();
        var accountName = GetAccountName();
        var accountKey = GetAccountKey();
        
        var connectionStringBody = GetConnectionStringByType(Types.Table, domain, accountName);

        if (partial) return connectionStringBody;

        var connectionStringHead = string.IsNullOrWhiteSpace(UserAssignedId)
            ? $"AccountName={accountName};AccountKey={accountKey};DefaultEndpointsProtocol={AzureEndpointProtocol};"
            : $"AccountName={accountName};DefaultEndpointsProtocol={LocalEndpointProtocol};";

        return $"{connectionStringHead}{connectionStringBody}";
    }

    public Uri GetFileEndpointUri(string containerName, string? dnsName = null)
    {
        var connectionName = dnsName is not null
            ? $"{LocalEndpointProtocol}://{dnsName}:{FilePort}/{GetAccountName()}/{containerName}"
            : IsUsingDevelopmentStorage
                ? $"{LocalEndpointProtocol}://127.0.0.1:{FilePort}/{GetAccountName()}/{containerName}"
                : $"{AzureEndpointProtocol}://{AccountName}.file.core.windows.net/{containerName}";

        return new Uri(connectionName);
    }
    
    public string GetFileConnectionString(bool partial = false)
    {
        var domain =  GetDnsName();
        var accountName = GetAccountName();
        var accountKey = GetAccountKey();
        
        var connectionStringBody = GetConnectionStringByType(Types.File, domain, accountName);

        if (partial) return connectionStringBody;

        var connectionStringHead = string.IsNullOrWhiteSpace(UserAssignedId)
            ? $"AccountName={accountName};AccountKey={accountKey};DefaultEndpointsProtocol={AzureEndpointProtocol};"
            : $"AccountName={accountName};DefaultEndpointsProtocol={LocalEndpointProtocol};";

        return $"{connectionStringHead}{connectionStringBody}";
    }

    /// <summary>
    /// Get a connecting string
    /// </summary>
    /// <remarks>
    /// connection string returned is either for azurite local/docker environment or for azure cloud.
    /// </remarks>
    /// <returns></returns>
    public string  GetConnectionString()
    {
        // The defaults provided represent azurite defaults
        // See https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio
        var accountName = GetAccountName();
        var accountKey = GetAccountKey();
        var protocol = IsUsingDevelopmentStorage ? LocalEndpointProtocol : AzureEndpointProtocol;
        
        var connectionStringHead = string.IsNullOrWhiteSpace(UserAssignedId)
            ? $"AccountName={accountName};AccountKey={accountKey};DefaultEndpointsProtocol={protocol};"
            : $"AccountName={accountName};DefaultEndpointsProtocol={protocol};";

        var stringBuilder = new StringBuilder();
        stringBuilder.Append(GetStorageConnectionString(true));
        stringBuilder.Append(GetQueueConnectionString(true));
        stringBuilder.Append(GetTableConnectionString(true));
        stringBuilder.Append(GetFileConnectionString(true));
        
        var connectionStringBody = stringBuilder.ToString();
        
#if  DEBUG
    using (_logger.BeginScope(nameof(GetConnectionString)))
    {
        _logger.LogDebug("Connection string head: {Head}", connectionStringHead);
        _logger.LogDebug("Connection string body: {Body}", connectionStringBody);
        _logger.LogDebug("Connection string full: {Head}{Body}", connectionStringHead, connectionStringBody);
    }    
#endif
        return $"{connectionStringHead}{connectionStringBody}";
    }
    
    /// <summary>
    /// Get token credential using manged identity or service principle
    /// </summary>
    /// <remarks>
    /// Using the managed identity requires the <see cref="UserAssignedId"/> to be set
    ///
    /// Using the service principle requires <see cref="TenantId" />, <see cref="ClientId"/>,
    /// and <see cref="ClientSecret"/> to be set
    /// </remarks>
    /// <returns></returns>
    public TokenCredential GetTokenCredential()
    {
        TokenCredential credential;

        if (string.IsNullOrWhiteSpace(UserAssignedId))
        {
            credential = new ClientSecretCredential(
                TenantId, 
                ClientId, 
                ClientSecret);
        }
        else
        {
            credential = new ChainedTokenCredential(
                new ManagedIdentityCredential(UserAssignedId),
                new AzureCliCredential());
        }

        return credential;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public bool IsUsingDevelopmentStorage => AccountName == DevelopmentConnectionString;

    private string GetConnectionStringByType(Types type, string domain, string accountName)
    {
        string endpointType;
        string connectionStringType;
        int port;

        switch (type)
        {
            case Types.Blob:
                endpointType = "BlobEndpoint";
                connectionStringType = "blob";
                port = StoragePort;
                break;
            
            case Types.Queue:
                endpointType = "QueueEndpoint";
                connectionStringType = "queue";
                port = QueuePort;
                break;
            
            case Types.Table:
                endpointType = "TableEndpoint";
                connectionStringType = "table";
                port = TablePort;
                break;
            
            case Types.File:
                endpointType = "FileEndpoint";
                connectionStringType = "file";
                port = FilePort;
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        var connectionStringBody = IsUsingDevelopmentStorage
            ? $"{endpointType}={LocalEndpointProtocol}://{domain}:{port}/{accountName};"
            : $"{endpointType}={AzureEndpointProtocol}://{AccountName}.{connectionStringType}.core.windows.net/;";

        return connectionStringBody;
    }

    private string GetDnsName() => DnsName ?? "127.0.0.1";
    
    private string GetAccountName() => IsUsingDevelopmentStorage 
        ? AzuriteDefaultAccountName 
        : string.IsNullOrWhiteSpace(AccountName) 
            ? AzuriteDefaultAccountName : AccountName;

    private string GetAccountKey() => AccountKey ?? AzuriteDefaultAccountKey;
}