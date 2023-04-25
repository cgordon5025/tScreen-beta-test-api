using System;

namespace Application.Common.Exceptions;

public class UnknownWebClientException : Exception
{
    // ReSharper disable once MemberCanBePrivate.Global
    public string ClientUrl { get; }
    
    public UnknownWebClientException(string clientUrl) 
        : base($"Unknown web client found {clientUrl}")
    {
        ClientUrl = clientUrl;
    }
}