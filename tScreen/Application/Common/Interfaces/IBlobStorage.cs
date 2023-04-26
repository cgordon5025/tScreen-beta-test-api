using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs.Models;

namespace Application.Common.Interfaces;

public interface IBlobStorage
{
    public string? StorageAccountName { get; set; }
    public string? LastUsedFileName { get; set; }

    public static string NewBlobName() => Guid.NewGuid().ToString();
    public static string NewContainerName() => Guid.NewGuid().ToString();

    /// <summary>
    /// Select storage account to interact with see <see cref="BlobStorageType"/> for
    /// available storage accounts
    /// </summary>
    /// <param name="type">Storage account type</param>
    /// <returns>Fluent interface</returns>
    public IBlobStorage SetStorageAccount(BlobStorageType type);

    public Task<Response<BlobContentInfo>> UploadBlobAsync(Stream content, string path,
        IDictionary<string, string> metaData = default!, bool overwrite = false);

    public Task<Response<BlobContentInfo>> UploadBlobAsync(Stream content, string path,
        string fileName, IDictionary<string, string> metaData = default!, bool overwrite = false);

    public Task<Response<BlobContentInfo>> UploadBlobWithNewNameAsync(Stream content, string path,
        IDictionary<string, string> metaData = default!, bool overwrite = false);

    /// <summary>
    /// Download blob file from blob storage
    /// </summary>
    /// <param name="path">Blob storage container name</param>
    /// <param name="blobName">Blob storage blob name</param>
    /// <returns>API response BlobDownloadInfo</returns>
    public Task<Response<BlobDownloadInfo>> DownloadAsync(string path, string blobName);
}

public enum BlobStorageType
{
    Client,
    Core
}