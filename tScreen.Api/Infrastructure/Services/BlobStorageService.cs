using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Core;
using Core.Settings;

namespace Infrastructure.Services;

public class BlobStorageService : IBlobStorage
{
    private readonly AzureBlobStorageSettings _blobStorageGroupSettings;
    private readonly IApplicationEnvironment _environment;
    private BlobServiceClient? _blobServiceClient;
        
    public string? StorageAccountName { get; set; }
    public string? LastUsedFileName { get; set; }
    
    public BlobStorageService(AzureBlobStorageSettings blobStorageSettings, IApplicationEnvironment environment)
    {
        _blobStorageGroupSettings = blobStorageSettings;
        _environment = environment;

        // If this API is running outside of Docker and running locally then set the 
        // DNSName to null so that the localhost (127.0.0.1) is used which is used 
        // to access Azurite (Blob Storage emulation) over a bridged network
        if (environment.IsLocallyHosted())
            _blobStorageGroupSettings.DnsName = null;
    }

    /// <summary>
    /// Select storage account to interact with see <see cref="BlobStorageType"/> for
    /// available storage accounts
    /// </summary>
    /// <param name="type">Storage account type</param>
    /// <returns>Fluent interface</returns>
    public IBlobStorage SetStorageAccount(BlobStorageType type)
    {
        _blobServiceClient = _createStorageAccountFromType(type);

        if (_blobServiceClient is null)
            throw new NullReferenceException("Blob service client reference is null");
        
        StorageAccountName = _blobServiceClient.AccountName;
        return this;
    }
    
    // ReSharper disable once MemberCanBePrivate.Global
    public static string NewBlobName() => Guid.NewGuid().ToString();
    
    // ReSharper disable once MemberCanBePrivate.Global
    public static string NewContainerName() => Guid.NewGuid().ToString();

    public async Task<Response<BlobContentInfo>> UploadBlobAsync(Stream content, string path, 
        IDictionary<string, string> metaData = default!, bool overwrite = false)
        => await _uploadBlobAsync(content, path, default!, metaData, overwrite);

    public async Task<Response<BlobContentInfo>> UploadBlobAsync(Stream content, string path,
        string fileName, IDictionary<string, string> metaData = default!, bool overwrite = false)
        => await _uploadBlobAsync(content, path, fileName, metaData, generateFileName: false, overwrite);

    public async Task<Response<BlobContentInfo>> UploadBlobWithNewNameAsync(Stream content, string path,
        IDictionary<string, string> metaData = default!, bool overwrite = false)
        => await _uploadBlobAsync(content, path, default!, metaData, generateFileName: true, overwrite);

    /// <summary>
    /// Download blob file from blob storage
    /// </summary>
    /// <param name="path">Blob storage container name</param>
    /// <param name="blobName">Blob storage blob name</param>
    /// <returns>API response BlobDownloadInfo</returns>
    public async Task<Response<BlobDownloadInfo>> DownloadAsync(string path, string blobName)
    {
        _hasSelectedStorageAccount();
            
        var (container, directory) = GetContainerAndDirectory(path);
        
        var blobContainerClient = _blobServiceClient!.GetBlobContainerClient(container);
        
        var fullyQualifiedName = directory + blobName;
        var blobClient = blobContainerClient.GetBlobClient(fullyQualifiedName);
        
        return await blobClient.DownloadAsync();
    }
    
    private async Task<Response<BlobContentInfo>> _uploadBlobAsync(Stream content, string path, string fileName, 
        IDictionary<string, string> metaData, bool generateFileName = false, bool overwrite = false)
    {
        _hasSelectedStorageAccount();

        LastUsedFileName = (generateFileName)
            ? NewBlobName()
            : fileName;

        var (container, directory) = GetContainerAndDirectory(path);
            
        var blobContainerClient = _blobServiceClient!.GetBlobContainerClient(container);
            
        // If the container exists a 409 conflict is returned, which is okay for now, but logs will be polluted :(
        // see for details https://github.com/Azure/azure-sdk-for-net/issues/109#issuecomment-335568358
        // Also watching because a better and efficient solution may arrive soon
        // see for details https://github.com/Azure/azure-sdk-for-net/issues/9758 
        await blobContainerClient.CreateIfNotExistsAsync();
            
        await blobContainerClient.SetMetadataAsync(metaData);

        var fullyQualifiedPath = !string.IsNullOrWhiteSpace(LastUsedFileName)
            ? $"{directory}{LastUsedFileName}"
            : directory;

        var blobClient = blobContainerClient.GetBlobClient(fullyQualifiedPath);
        
        return await blobClient.UploadAsync(content, overwrite);
    }

    protected static (string, string) GetContainerAndDirectory(string path)
    {
        var parts = path.Split("/");
        var container = parts[0];
        var directory = parts.Length > 0 
            ? $"{string.Join("/", parts[1..])}/"
            : "";
            
        return (container, directory);
    }
    
    /// <summary>
    /// Select storage account from supported list
    /// </summary>
    /// <param name="type">Storage account type</param>
    /// <returns>Fluent Interface</returns>
    /// <exception cref="NotSupportedException">Only supports General for the time being</exception>
    private BlobServiceClient _createStorageAccountFromType(BlobStorageType type)
    {
        if (_blobStorageGroupSettings.IsUsingDevelopmentStorage)
            return new BlobServiceClient(_blobStorageGroupSettings.GetConnectionString());
                
        return type switch
        {
            BlobStorageType.Client =>
                !string.IsNullOrWhiteSpace(_blobStorageGroupSettings.UserAssignedId)
                    ? new BlobServiceClient(_blobStorageGroupSettings.GetStorageEndpointUri(""), 
                        _blobStorageGroupSettings.GetTokenCredential())
                    : new BlobServiceClient(_blobStorageGroupSettings.GetConnectionString()),
            BlobStorageType.Core => 
                throw new NotSupportedException("Not implemented yet"),
            _ => throw new NotSupportedException("")
        };
    }
    
    private void _hasSelectedStorageAccount()
    {
        if (_blobServiceClient == null)
            throw new ArgumentNullException(nameof(_blobServiceClient), 
                "Storage account is not specified. Use SetStorageAccount to select a storage account");
    }
}