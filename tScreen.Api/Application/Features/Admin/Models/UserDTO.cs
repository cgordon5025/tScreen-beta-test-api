using System;
using Application.Common.Models;

namespace Application.Features.Admin.Models;

public class UserDTO : BaseEntityDTO
{
    public Guid CompanyId { get; set; }
    public string UserName { get; set; } = null!;
    public string NormalizedUserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string NormalizedEmail { get; set; } = null!;
    public bool EmailConfirmed { get; set; }
    public DateTime LastSignedIn { get; set; }
    public PersonDTO? Person { get; set; }
}