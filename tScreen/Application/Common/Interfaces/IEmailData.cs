using System;

namespace Application.Common.Interfaces;

public interface IEmailData
{
    public string Fullname { get; init; }
    public DateTime CurrentDate { get; init; }
}