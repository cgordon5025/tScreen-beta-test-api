namespace GraphQl.Models;

using System.ComponentModel.DataAnnotations;

public class ResetPasswordRequest
{
    [Required] public string Email { get; set; } = null!;
    [Required] public string Token { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
    [Required] public string ConfirmPassword { get; set; } = null!;
}