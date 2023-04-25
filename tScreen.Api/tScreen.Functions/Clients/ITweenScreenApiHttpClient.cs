using System;
using System.Threading.Tasks;

namespace TweenScreen.Functions.Clients;

public interface ITweenScreenApiHttpClient
{
    public Uri BaseAddress { get; }
    
    void SetAccessToken(string token);
    void SetTimeout(TimeSpan timeout);
    Task<bool> AddWorkLists(Guid sessionId);
    Task<bool> NotifySessionWorklistAssociatedPersons(Guid sessionId);
}