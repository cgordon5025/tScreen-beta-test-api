using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Models;
using Application.Features.Admin.Models;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Common.Interfaces;

public interface IIdentityService
{
    public Task<User?> CreateAccount(PersonDTO person,
        string email, string password, IEnumerable<string> roles, IEnumerable<RoleClaim> claims);
    public Task<AuthorizeResultDTO> Authenticate(string email, string password);
    public Task<bool> Authorize(Guid userId, string policyName);
    public Task<IdentityResult?> ChangePassword(Guid id, string oldPassword, string newPassword);
    public Task<bool> ForgotPassword(string email, string clientUrl);
    public Task<bool> ResetPassword(string token, string email, string password);
    public Task<bool> ConfirmPassword(string email, string password);
    public Task<bool> ActivateAccount(Guid id, string token);
    public Task<bool> ArchiveAccount(Guid id);
    public Task<bool> RemoveAccount(Guid id);
}