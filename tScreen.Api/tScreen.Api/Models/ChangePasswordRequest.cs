using System;
using System.ComponentModel.DataAnnotations;

namespace GraphQl.Models;

public class ChangePasswordRequest
{
    [Required] public Guid Id { get; init;  } 
    [Required] public string OldPassword { get; init; } = null!;
    [Required] public string NewPassword { get; init; } = null!;
    [Required] public string ConfirmPassword { get; init; } = null!;
}