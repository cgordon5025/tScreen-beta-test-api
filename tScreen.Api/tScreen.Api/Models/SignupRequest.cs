using System;
using System.ComponentModel.DataAnnotations;

namespace GraphQl.Models;

public class SignupRequest
{
    [Required]
    public Guid CompanyId { get; set; }

    [Required]
    public string FirstName { get; set; } = null!;
    
    public string? MiddleName { get; set; }
    
    [Required]
    public string LastName { get; set; } = null!;
    
    [Required, EmailAddress] 
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}