using System;

namespace Application.Events;

public class PasswordResetEvent : BaseEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; }
    public string Fullname { get; set; }

    public PasswordResetEvent(string fullName, string email, string token)
    {
        Fullname = fullName;
        Email = email;
    }
}