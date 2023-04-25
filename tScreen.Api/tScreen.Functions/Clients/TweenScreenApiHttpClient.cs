using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace TweenScreen.Functions.Clients;

public class TweenScreenApiHttpClient : ITweenScreenApiHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly TweenScreenApiSettings _settings;
    
    public Uri BaseAddress { get; }

    public TweenScreenApiHttpClient(HttpClient httpClient, TweenScreenApiSettings settings, 
        ILogger<TweenScreenApiHttpClient> logger)
    {
        _httpClient = httpClient;
        _settings = settings;
        
        _setupDefaults(_settings);
        
        logger.LogInformation("Connecting to API {BaseApiUrl}", _settings.BaseUrl);
        BaseAddress = _httpClient.BaseAddress;
    }

    public void SetAccessToken(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public void SetTimeout(TimeSpan timeout)
    {
        _httpClient.Timeout = timeout;
    }

    public async Task<bool> AddWorkLists(Guid sessionId)
    {
        var request = new { sessionId };
        
        var json = Utility.SerializeObject(request);
        var payload = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
        
        var response = await _httpClient.PostAsync("/api/report/create", payload);

        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> NotifySessionWorklistAssociatedPersons(Guid sessionId)
    {
        var request = new { sessionId };
        
        var json = Utility.SerializeObject(request);
        var payload = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
        
        var response = await _httpClient.PostAsync("/api/worklist/notify", payload);

        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

    /**
     * Protected/private methods
     */
    private void _setupDefaults(TweenScreenApiSettings setting)
    {
        _httpClient.BaseAddress = _getBaseUrl(setting);
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, nameof(TweenScreenApiHttpClient));
    }

    private Uri _getBaseUrl(TweenScreenApiSettings setting)
    {
        var uriBuilder = new UriBuilder(new Uri(setting.BaseUrl, UriKind.Absolute));
        return uriBuilder.Uri;
    }
}