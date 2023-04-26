using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Admin.Models;
using Azure.Core;
using Core.Settings;
using Domain.Entities.Identity;
using GraphQl.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GraphQl.Controllers;

[Authorize, ApiController, Route("api/oauth")]
public class OAuthController : Controller
{
    private readonly IIdentityService _identityService;
    private readonly KnownWebClientsSettings _knownWebClientsSettings;
    private readonly ILogger<OAuthController> _logger;

    public OAuthController(
        IIdentityService identityService,
        KnownWebClientsSettings knownWebClientsSettings,
        ILogger<OAuthController> logger)
    {
        _identityService = identityService;
        _knownWebClientsSettings = knownWebClientsSettings;
        _logger = logger;
    }

    /// <summary>
    /// Used with OAuth supporting tools like Postman
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("authenticate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthenticateResponse>> Authenticate([FromForm] AuthenticateRequest responseModel)
    {
        var result = await _identityService.Authenticate(responseModel.Username, responseModel.Password);
        if (!result.Success)
            return Unauthorized(new AuthenticateResponse
            {
                Message = result.Message!,
                Status = StatusCodes.Status401Unauthorized,
                ResponseType = result.Status!
            });

        return Ok(new AuthenticateResponse
        {
            Message = result.Message!,
            Status = StatusCodes.Status200OK,
            AccessToken = result.Token!,
            TokenType = "bearer",
            ExpiresIn = result.Expires.ToString("##")
        });
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest requestModel)
    {
        try
        {
            await _identityService.ForgotPassword(requestModel.Email, requestModel.ClientUrl);
        }
        catch (UnknownWebClientException ex)
        {
            return BadRequest(new { ex.ClientUrl, ex.Message });
        }

        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest requestModel)
    {
        if (requestModel.Password != requestModel.ConfirmPassword)
            return BadRequest(new { Message = "Password doesn't match" });

        var result = await _identityService
            .ResetPassword(requestModel.Token, requestModel.Email, requestModel.Password);

        if (!result)
            return BadRequest(new { Message = "Cannot reset password due to application error" });

        return Ok();
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest requestModel)
    {
        if (requestModel.NewPassword != requestModel.ConfirmPassword)
            return BadRequest(new { Message = "New and confirmed passwords do not match" });

        var result = await _identityService
            .ChangePassword(requestModel.Id, requestModel.OldPassword, requestModel.NewPassword);

        return result is not null && result.Succeeded
            ? Ok() : BadRequest(new { Message = "Invalid details password cannot be changed" });
    }

    [AllowAnonymous]
    [HttpPost("signup")]
    public async Task<ActionResult<SignupResponse>> Signup([FromBody] SignupRequest requestModel)
    {
        var rolesClaims = new List<RoleClaim>()
        {
            new()
            {
                ClaimType = nameof(UserClaimTypes.Scope),
                ClaimValue = "user.read"
            },
            new()
            {
                ClaimType = nameof(UserClaimTypes.Scope),
                ClaimValue = "user.update"
            },
            new()
            {
                ClaimType = nameof(UserClaimTypes.Scope),
                ClaimValue = "user.delete"
            },
            new()
            {
                ClaimType = nameof(UserClaimTypes.Scope),
                ClaimValue = "user.archive"
            },
        };

        var personDTO = new PersonDTO
        {
            CompanyId = requestModel.CompanyId,
            FirstName = requestModel.FirstName,
            MiddleName = requestModel.MiddleName,
            LastName = requestModel.LastName,
        };

        var user = await _identityService.CreateAccount(personDTO,
            requestModel.Email, requestModel.Password, new[] { "admin" }, rolesClaims);

        if (user is null)
            return BadRequest();

        return Ok(new SignupResponse
        {
            Id = user.Id,
            CompanyId = user.CompanyId,
            CreatedAt = user.CreatedAt
        });
    }

    [AllowAnonymous]
    [HttpPost("signup-test-users")]
    public async Task<ActionResult<IEnumerable<User>>> SignupTestUsers([FromBody] SignupRequest requestModel)
    {
        var rolesClaims = new List<RoleClaim>()
        {
            new()
            {
                ClaimType = nameof(UserClaimTypes.Scope),
                ClaimValue = "user.read"
            },
            new()
            {
                ClaimType = nameof(UserClaimTypes.Scope),
                ClaimValue = "user.update"
            },
            new()
            {
                ClaimType = nameof(UserClaimTypes.Scope),
                ClaimValue = "user.delete"
            },
            new()
            {
                ClaimType = nameof(UserClaimTypes.Scope),
                ClaimValue = "user.archive"
            },
        };

        const string userPassword = "Testing1234$";

        var personList = new List<PersonUserData>()
        {
            new ("Nikola", "Tesla", "test@test.com", new [] { "owner" }),
            new ("Albert", "Einstein", "admin.test@test.com", new [] { "admin" }),
            new ("Ada", "Lovelace", "user.test@test.com", new [] { "user" }),
            new ("Thomas", "Edison", "player.test@test.com", new [] { "player" }),
            new ("John", "Doe", "all.test@test.com", new [] { "admin", "user", "player" })
        };

        var users = new List<User?>();
        foreach (var person in personList)
            users.Add(await _identityService.CreateAccount(new PersonDTO
            {
                CompanyId = requestModel.CompanyId,
                FirstName = person.FirstName,
                LastName = person.LastName,
            },
                person.Email, userPassword, person.Roles, rolesClaims));

        return Ok(users);
    }

    class PersonUserData
    {
        public PersonUserData(string firstName, string lastName, string email, IEnumerable<string> roles)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Roles = roles;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}