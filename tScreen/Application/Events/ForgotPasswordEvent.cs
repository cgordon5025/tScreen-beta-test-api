using System;

namespace Application.Events;

public class ForgotPasswordEvent : BaseEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Fullname { get; }
    public string Email { get; }
    public string Token { get; }
    public string ClientUrl { get; }

    public ForgotPasswordEvent(string fullName, string email, string token, string clientUrl)
    {
        Fullname = fullName;
        Email = email;
        Token = token;
        ClientUrl = clientUrl;
    }
}