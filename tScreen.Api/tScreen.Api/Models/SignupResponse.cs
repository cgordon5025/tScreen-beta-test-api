using System;

namespace GraphQl.Models;

public class SignupResponse
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public DateTime CreatedAt { get; set; }
}