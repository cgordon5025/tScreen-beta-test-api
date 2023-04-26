using System;
using System.ComponentModel.DataAnnotations;

namespace GraphQl.Models.SessionController;

public class AuthenticateRequest
{
    [Required]
    public Guid SessionId { get; set; }

    [Required]
    public Guid StudentId { get; set; }

    [Required]
    public string Code { get; set; } = null!;

    [Required]
    public DateTime Dob { get; set; }

    [Required]
    public int GradeLevel { get; set; }
}