using System;

namespace Application.Events;

public class ChangedPasswordEvent : BaseEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; }
    public string Fullname { get; set; }

    public ChangedPasswordEvent(string fullName, string email)
    {
        Fullname = fullName;
        Email = email;
    }
}