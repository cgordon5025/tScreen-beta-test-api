using System;
using System.Threading.Tasks;
using Application.Common;
using Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TweenScreen.Functions.Clients;

namespace TweenScreen.Functions;

public class SessionFunctions
{
    private readonly ILogger<SessionFunctions> _logger;
    private readonly ITweenScreenApiHttpClient _apiHttpClient;
    private readonly ITokenProvider _tokenProvider;

    public SessionFunctions(ILogger<SessionFunctions> logger, ITweenScreenApiHttpClient apiHttpClient,
        ITokenProvider tokenProvider)
    {
        _logger = logger;
        _apiHttpClient = apiHttpClient;
        _tokenProvider = tokenProvider;

        _apiHttpClient.SetTimeout(TimeSpan.FromMinutes(30));
    }
    
    [FunctionName(nameof(ProcessPendingSessions))]
    public async Task ProcessPendingSessions([QueueTrigger(StorageQueues.CreateReport)] string queueItem)
    {
        _logger.LogInformation("Running {FunctionName} with payload \"{Payload}\" and sending to {ApiBaseUrl}",
            nameof(ProcessPendingSessions), queueItem, _apiHttpClient.BaseAddress);   

        var message = Utility.DeserializeObject<PendingWorkListMessage>(queueItem);

        await SetAccessToken();
        await _apiHttpClient.AddWorkLists(message.SessionId);
        
        await Task.Delay(TimeSpan.FromSeconds(3));
    }

    [FunctionName(nameof(SessionReportCreated))]
    public async Task SessionReportCreated([QueueTrigger(StorageQueues.CreateWorklist)] string queueItem)
    {
        _logger.LogInformation("Running {FunctionName} with payload \"{Payload}\" and sending to {ApiBaseUrl}",
            nameof(SessionReportCreated), queueItem, _apiHttpClient.BaseAddress);   
        
        var message = Utility.DeserializeObject<PendingWorkListMessage>(queueItem);

        await SetAccessToken();
        await _apiHttpClient.NotifySessionWorklistAssociatedPersons(message.SessionId);
        
        await Task.Delay(TimeSpan.FromSeconds(1));
    }

    private async Task SetAccessToken()
    {
        var authResult = await _tokenProvider.GetAzureAdToken();
        _apiHttpClient.SetAccessToken(authResult.AccessToken);
    }
}