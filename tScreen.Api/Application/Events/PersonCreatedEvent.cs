using System;
using Domain.Entities;

namespace Application.Events;

public class PersonCreatedEvent : BaseEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public Person Entity { get; }
    public string Email { get; }
    public string? Password { get; }
    
    public PersonCreatedEvent(Person person, string email, string? password)
    {
        Entity = person;
        Email = email;
        Password = password;
    }
}