using System;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Interfaces;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Queues;
using Core;
using Core.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Infrastructure.Services;

public class QueueService : IQueueService
{
    private readonly QueueClient _createReportQueue;
    private readonly QueueClient _createReportQueuePoison;
    private readonly QueueClient _createWorkListQueue;
    private readonly QueueClient _createWorkListQueuePoison;
    private readonly ILogger<QueueService> _logger;
    
    public QueueService(IApplicationEnvironment environment, ILogger<QueueService>? logger, 
        AzureBlobStorageSettings blobStorageSettings)
    {
        // If this API is running outside of Docker and running locally then set the 
        // DNSName to null so that the localhost (127.0.0.1) is used which is used 
        // to access Azurite (Blob Storage emulation) over a bridged network
        if (environment.IsLocallyHosted())
            blobStorageSettings.DnsName = null;
        
        _logger = logger ?? NullLogger<QueueService>.Instance;
        
        var options = new QueueClientOptions()
        {
            MessageEncoding = QueueMessageEncoding.Base64
        };

        var reportQueue = StorageQueues.CreateReportQueue.Name;
        var reportQueuePoison = $"{reportQueue}-poison";
        var workListQueue = StorageQueues.CreateWorkListQueue.Name;
        var workListQueuePoison = $"{workListQueue}-poison";

        // if (environment.IsContainerHosted())
        // {
            _logger.LogDebug("DNS Name {DsnName}", blobStorageSettings.DnsName);
            _logger.LogDebug("Account Name {AccountName}", blobStorageSettings.AccountName);
            _logger.LogDebug("Using Development Storage {IsUsingDevelopmentStorage}", blobStorageSettings.IsUsingDevelopmentStorage);
            _logger.LogDebug("Report queue (1): {ReportUrl}", reportQueue);
            _logger.LogDebug("Report queue (2): {ReportUrl}", reportQueuePoison);
            _logger.LogDebug("WorkList queue (1): {ReportUrl}", workListQueue);
            _logger.LogDebug("WorkList queue (2): {ReportUrl}", workListQueuePoison);
        // }

        if (environment.GetEnvironmentVariableOrDefault(
                EnvironmentVariableNames.ApplicationUseDevelopmentStorage, false) || 
            string.IsNullOrWhiteSpace(blobStorageSettings.UserAssignedId))
        {
            var connectionString = blobStorageSettings.GetQueueConnectionString();
            
            _createReportQueue = new QueueClient(connectionString, reportQueue, options);
            _createReportQueuePoison = new QueueClient(connectionString, reportQueuePoison, options);
            
            _createWorkListQueue = new QueueClient(connectionString, workListQueue, options);
            _createWorkListQueuePoison = new QueueClient(connectionString, workListQueuePoison, options);
        }
        else
        {
            var credential = blobStorageSettings.GetTokenCredential();
            var reportQueueUri = blobStorageSettings.GetQueueEndpointUri(reportQueue);
            var reportQueuePoisonUri = blobStorageSettings.GetQueueEndpointUri(reportQueuePoison);
            var workListQueueUri = blobStorageSettings.GetQueueEndpointUri(workListQueue);
            var workListQueuePoisonUri = blobStorageSettings.GetQueueEndpointUri(workListQueuePoison);
            
            _logger.LogInformation("Using managed identity {Msi}", blobStorageSettings.UserAssignedId);
        
            _logger.LogDebug("Report queue URI {Uri}", reportQueueUri);
            _logger.LogDebug("Report poison queue URI {Uri}", reportQueuePoisonUri);
            _logger.LogDebug("WorkList queue URI {Uri}", workListQueueUri);
            _logger.LogDebug("WorkList poison queue URI {Uri}", workListQueuePoisonUri);
        
            _createReportQueue = new QueueClient(reportQueueUri, credential, options);
            _createReportQueuePoison = new QueueClient(reportQueuePoisonUri, credential, options);
        
            _createWorkListQueue = new QueueClient(workListQueueUri, credential, options);
            _createWorkListQueuePoison = new QueueClient(workListQueuePoisonUri, credential, options);
        }

        // Ensure both queues are created
        CreateQueuesIfDontExist().GetAwaiter();
    }
    
    /// <summary>
    /// Send message to queue
    /// </summary>
    /// <param name="message">Serialized JSON object</param>
    /// <param name="queue">Queue type</param>
    /// <exception cref="ArgumentException">Message is required. See <param name="message"></param></exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Throws when a Queue type has not been factored in method call
    /// </exception>
    public async Task SendMessageAsync(string message, StorageQueues queue)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message must have a body", nameof(message));
        
        _logger.LogInformation("Sending Message {Type}", queue.Name);

        switch (queue.Name)
        {
            case StorageQueues.CreateReport:
                await _createReportQueue.SendMessageAsync(message).ConfigureAwait(false);
                break;
            
            case StorageQueues.CreateWorklist:
                await _createWorkListQueue.SendMessageAsync(message).ConfigureAwait(false);
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(queue), queue.Name, null);
        }
    }

    /// <summary>
    /// Get message fom queue
    /// </summary>
    /// <remarks>
    /// Attempt to get message from queue. If the message (in JSON) is malformed
    /// push malformed message to parallel poison queue.
    /// </remarks>
    /// <param name="queue">Queue type</param>
    /// <typeparam name="TEntity">Message model</typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Throws when a Queue type has not been factored in method call
    /// </exception>
    public async Task<TEntity?> GetNextMessageAsync<TEntity>(StorageQueues queue) where TEntity : class, new()
    {
        var response = queue.Name switch
        {
            StorageQueues.CreateReport => await _createReportQueue.ReceiveMessageAsync(),
            StorageQueues.CreateWorklist => await _createWorkListQueue.ReceiveMessageAsync(),
            _ => throw new ArgumentOutOfRangeException(nameof(queue), queue.Name, null)
        };
        
        if (response?.Value is null)
            return null;

        try
        {
            var payload = response.Value.Body.ToObjectFromJson<TEntity>();
            return payload;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Received message appears malformed because it failed to deserialize " +
                                 "moving to {PoisonQueueName}"
                                , $"{queue}-poison");

            switch (queue.Name)
            {
                case StorageQueues.CreateReport:
                    await _createReportQueuePoison.SendMessageAsync(response.Value.Body);
                    await _createReportQueue.DeleteMessageAsync(response.Value.MessageId, response.Value.PopReceipt);
                    break;
                
                case StorageQueues.CreateWorklist:
                    await _createWorkListQueuePoison.SendMessageAsync(response.Value.Body);
                    await _createWorkListQueue.DeleteMessageAsync(response.Value.MessageId, response.Value.PopReceipt);
                    break;
                
                default:
                    _logger.LogCritical("Cannot move bad message to poison queue because {QueueName} is not supported", 
                        queue.Name);
                    break;
            }
        }

        return null;
    }
    
    private async Task CreateQueuesIfDontExist()
    {
        await _createReportQueue.CreateIfNotExistsAsync();
        await _createReportQueuePoison.CreateIfNotExistsAsync();
        await _createWorkListQueue.CreateIfNotExistsAsync();
        await _createWorkListQueuePoison.CreateIfNotExistsAsync();
    }
}