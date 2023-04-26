using System;
using System.Collections.Generic;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Events;
using Application.Features.Admin.Models;
using Application.Features.Admin.Person.Commands;
using Application.Features.Admin.Person.Queries;
using Core;
using Core.Settings;
using Domain.Entities;
using Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IMediator _mediator;
    private readonly KnownWebClientsSettings _knownWebClientsSettings;
    private readonly ILogger<IdentityService> _logger;

    public IdentityService(UserManager<User> userManager, RoleManager<Role> roleManager,
        SignInManager<User> signInManager, ITokenService tokenService, IMediator mediator,
        KnownWebClientsSettings knownWebClientsSettings,
        ILogger<IdentityService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _mediator = mediator;
        _knownWebClientsSettings = knownWebClientsSettings;
        _logger = logger;
    }

    public async Task<AuthorizeResultDTO> Authenticate(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentNullException(nameof(email));

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentNullException(nameof(password));

        var authorizeResultDTO = new AuthorizeResultDTO
        {
            Message = "Signin failed. Email or password is incorrect",
            Success = false,
            Status = nameof(AuthorizeResultStatusTypes.InvalidCredentials)
        };

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return authorizeResultDTO;

        var authResult = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!authResult.Succeeded)
            return authorizeResultDTO;

        if (authResult.IsLockedOut)
        {
            authorizeResultDTO.Message = "Account is locked";
            authorizeResultDTO.Status = nameof(AuthorizeResultStatusTypes.LockedOut);
            return authorizeResultDTO;
        }

        if (!user.EmailConfirmed)
        {
            authorizeResultDTO.Message = "Account not activated";
            authorizeResultDTO.Status = nameof(AuthorizeResultStatusTypes.NotVerified);
            return authorizeResultDTO;
        }

        // We convert user claims into scopes which will be injected into the built JWT.
        var scopes = (await _userManager.GetClaimsAsync(user))
            .Where(e => e.Type == nameof(UserClaimTypes.Scope))
            .Select(x => x.Value);

        var roles = await _userManager.GetRolesAsync(user);

        var personDTO = await _mediator.Send(new GetPersonByIdentityId { IdentityId = user.Id });

        var locationId = personDTO.LocationPersons
            .FirstOrDefault(e => e.Type == LocationTypes.Default)?.LocationId ?? Guid.Empty;

        var tokenClaims = _tokenService
            .BuildClaims(user, personDTO.Id, locationId, roles, scopes);

        var token = _tokenService.BuildToken(tokenClaims);

        authorizeResultDTO.Message = "Successfully signed in";
        authorizeResultDTO.Token = new JwtSecurityTokenHandler().WriteToken(token);
        authorizeResultDTO.Status = nameof(AuthorizeResultStatusTypes.Success);
        authorizeResultDTO.Expires = (token.ValidTo - DateTime.UtcNow).TotalSeconds;
        authorizeResultDTO.Success = true;

        var currentDateTime = DateTime.UtcNow;
        user.LastSignedIn = currentDateTime;
        user.UpdatedAt = currentDateTime;

        await _userManager.UpdateAsync(user);

        return authorizeResultDTO;
    }

    public Task<bool> Authorize(Guid userId, string policyName)
    {
        throw new NotImplementedException();
    }

    public async Task<User> CreateAccount(
        PersonDTO person,
        string email,
        string password,
        IEnumerable<string> roles,
        IEnumerable<RoleClaim> roleClaims)
    {
        if (person is null)
            throw new ArgumentNullException(nameof(person));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentNullException(nameof(email));

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentNullException(nameof(password));

        var rolesList = roles.ToList();

        // Check if the roles provided do exist, if not exit early
        if (rolesList.Any())
            foreach (var role in rolesList)
                if (!await _roleManager.RoleExistsAsync(role))
                    throw new Exception($"Role {role} does not exist");

        var user = new User
        {
            CompanyId = person.CompanyId,
            Email = email,
            NormalizedEmail = email.ToUpper(),
            UserName = email,
            NormalizedUserName = email.ToUpper(),
            LockoutEnabled = false,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            throw new Exception($"Unable to create new account. Error {result.Errors}");

        person.IdentityId = user.Id.ToString();
        person.IdentityType = "IdentityFramework";
        await _mediator.Send(new AddPerson { PersonDTO = person });

        // If roles are present bind to user
        // Note: intentionally separate from validation/exit early logic above
        if (rolesList.Any())
            await _userManager.AddToRolesAsync(user, rolesList);

        var claims = roleClaims.Select(roleClaim =>
            new Claim(roleClaim.ClaimType, roleClaim.ClaimValue));

        await _userManager.AddClaimsAsync(user, claims);

        return result.Succeeded ? user : null;
    }

    public async Task<IdentityResult?> ChangePassword(Guid id, string oldPassword, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(oldPassword))
            throw new ArgumentNullException(nameof(oldPassword));

        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentNullException(nameof(newPassword));

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
            return null;

        var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        if (!result.Succeeded) return result;

        var personDTO = await _mediator.Send(new GetPersonByIdentityId { IdentityId = user.Id });

        var fullName = $"{personDTO.FirstName} {personDTO.LastName}";
        await _mediator.Publish(new ChangedPasswordEvent(fullName, user.Email));

        return result;
    }

    /// <summary>
    /// Attempt to reset the user's password. Note: does not actually reset the user's password but instead
    /// notifies them via email of the attempt and provides a callback URL to reset their password
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <param name="clientUrl">
    /// URL of the client triggering the request. Used for the callback URL embedded into email sent to the user
    /// with notification of the attempt
    /// </param>
    /// <returns></returns>
    public async Task<bool> ForgotPassword(string email, string clientUrl)
    {
        var uri = new Uri(clientUrl);
        var origin = uri.GetComponents(
            UriComponents.Scheme | UriComponents.Host | UriComponents.Port, UriFormat.UriEscaped);

        if (!_knownWebClientsSettings.Domains.ToArray().Contains(origin))
        {
            _logger.LogWarning("Attempt to use ForgotPassword service with unknown web client {Client}", clientUrl);
            throw new UnknownWebClientException(clientUrl);
        }

        // Just encase the client URL provided includes a query string, we ignore it.
        var normalizedClientUrl = uri.GetComponents(
            UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path, UriFormat.UriEscaped);

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            _logger.LogWarning("User by {Email} does not exist", email);
            return false;
        }

        var personDTO = await _mediator.Send(new GetPersonByIdentityId { IdentityId = user.Id });
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var fullName = $"{personDTO.FirstName} {personDTO.LastName}";

        await _mediator.Publish(new ForgotPasswordEvent(fullName, email, token, normalizedClientUrl));

        return true;
    }

    public async Task<bool> ResetPassword(string token, string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        var personDTO = await _mediator.Send(new GetPersonByIdentityId { IdentityId = user.Id });

        _logger.LogInformation("Attempting to reset password for {Id}", user.Id);

        var decodedToken = token.Base64Decode();
        if (string.IsNullOrEmpty(decodedToken))
            throw new Exception("Cannot find reset token therefor cannot reset password");

        var result = await _userManager.ResetPasswordAsync(user, decodedToken, password);
        if (!result.Succeeded)
        {
            _logger.LogInformation("Failed to reset password for {Id}", user.Id);
            return false;
        }

        await _userManager.SetLockoutEnabledAsync(user, false);
        var fullName = $"{personDTO.FirstName} {personDTO.LastName}";
        await _mediator.Publish(new ChangedPasswordEvent(fullName, email));

        _logger.LogInformation("Successfully reset password for {Id}", user.Id);

        return true;
    }

    public async Task<bool> ConfirmPassword(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("User ID cannot be an empty Guid", nameof(email));

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentNullException(nameof(password));

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return false;

        _logger.LogInformation("{Id} Confirmed Password", user.Id);

        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<bool> ActivateAccount(Guid id, string token)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("User ID cannot be an empty Guid", nameof(id));

        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentNullException(nameof(token));

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
            return false;

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded) return false;

        var currDateTime = DateTime.UtcNow;
        user.UpdatedAt = currDateTime;

        await _userManager.UpdateAsync(user);
        return true;
    }

    public async Task<bool> ArchiveAccount(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("User ID cannot be an empty Guid", nameof(id));

        var currentDateTime = DateTime.UtcNow;
        var user = await _userManager.FindByIdAsync(id.ToString());
        user.UpdatedAt = currentDateTime;
        user.ArchivedAt = currentDateTime;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> RemoveAccount(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("User ID cannot be an empty Guid", nameof(id));

        var currentDateTime = DateTime.UtcNow;
        var user = await _userManager.FindByIdAsync(id.ToString());
        user.DeletedAt = currentDateTime;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }
}