using System.ComponentModel.DataAnnotations;

namespace GraphQl.Models;

public class ForgotPasswordRequest
{
    [Required]
    public string ClientUrl { get; set; } = null!;
    
    [Required]
    public string Email { get; set; } = null!;
}