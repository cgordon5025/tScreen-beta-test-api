using System;
using System.Threading.Tasks;

namespace tScreen.Functions.Clients;

public interface ItScreenApiHttpClient
{
    public Uri BaseAddress { get; }

    void SetAccessToken(string token);
    void SetTimeout(TimeSpan timeout);
    Task<bool> AddWorkLists(Guid sessionId);
    Task<bool> NotifySessionWorklistAssociatedPersons(Guid sessionId);
}