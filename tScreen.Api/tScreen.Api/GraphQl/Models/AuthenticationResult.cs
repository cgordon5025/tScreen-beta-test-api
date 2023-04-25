using System;

namespace GraphQl.GraphQl.Models;

public class AuthenticationResult
{
    public string? Message { get; set; }
    public string? AccessToken { get; set; }
    public string? Expires { get; set; }
    public string? Status { get; set; }
    public bool Success { get; set; }
}