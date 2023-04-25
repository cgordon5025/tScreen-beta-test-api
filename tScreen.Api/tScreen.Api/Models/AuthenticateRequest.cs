using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace GraphQl.Models;

public class AuthenticateRequest
{
    [Required] 
    public string Username { get; set; } = null!;

    [Required] 
    public string Password { get; set; } = null!;


    [Required, JsonProperty("grant_type")] 
    public string? GrantType { get; set; } = "password";
}