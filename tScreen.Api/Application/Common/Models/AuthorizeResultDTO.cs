namespace Application.Common.Models;

public class AuthorizeResultDTO
{
    /// <summary>
    /// Authorization result message
    /// </summary>
    /// <example>Successfully signed in</example>
    public string Message { get; set; } = null!;
    
    /// <summary>
    /// JWT access token
    /// </summary>
    public string? Token { get; set; }
    
    /// <summary>
    /// JWT Refresh token
    /// </summary>
    public string? RefreshToken { get; set; }
    
    /// <summary>
    /// When authorization expires
    /// </summary>
    /// <example>3600</example>
    public double Expires { get; set; }
    
    /// <summary>
    /// Status of the authorization attempt. See <see cref="AuthorizeResultStatusTypes"/> for all status types
    /// </summary>
    /// <example>Success</example>
    public string? Status { get; set; }
    
    /// <summary>
    /// If the authorization attempt was a success or not
    /// </summary>
    /// <example>True</example>
    public bool Success { get; set; }
}

public enum AuthorizeResultStatusTypes
{
    InvalidCredentials,
    NotVerified,
    LockedOut,
    Success
}