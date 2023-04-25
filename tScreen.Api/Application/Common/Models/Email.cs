using Application.Common.Interfaces;

namespace Application.Common.Models;

public class Email<T> where T : IEmailData
{
    public DataAddress? From { get; init; }
    public DataAddress To { get; init; } = null!;
    public string Subject { get; init; } = null!;
    public T Data { get; init; } = default!;
}

public class DataAddress
{
    public string Address { get; }
    public string Name { get; }

    public DataAddress(string address, string name)
    {
        Address = address;
        Name = name;
    }
}