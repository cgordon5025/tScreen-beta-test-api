using Newtonsoft.Json;

namespace GraphQl.Models;

public class AuthenticateResponse
{
    public string Message { get; set; } = null!;

    public int Status { get; set; }
    
    [JsonProperty("response_type")]
    public string? ResponseType { get; set; }

    [JsonProperty("access_token")]
    public string AccessToken { get; set; } = null!;
    
    [JsonProperty("token_type")]
    public string TokenType { get; set; } = null!;

    [JsonProperty("expires_in")] 
    public string ExpiresIn { get; set; } = null!;
}