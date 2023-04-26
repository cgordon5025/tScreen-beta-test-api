using System;
using System.Security.Principal;
using Domain.Inferfaces;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Identity;

public class Role : IdentityRole<Guid>, IEntity
{
    public string? Description { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}