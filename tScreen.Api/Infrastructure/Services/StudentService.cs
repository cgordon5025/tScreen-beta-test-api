using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Models;
using Application.Features.Admin.Session.Queries;
using Core.Settings.Models;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Infrastructure.Services;

public class StudentService
{
    private readonly IMediator _mediator;
    private readonly JwtSettings _settings;

    public StudentService(IMediator mediator, JwtSettings settings)
    {
        _mediator = mediator;
        _settings = settings;
    }
    
    public async Task<AuthorizeResultDTO> AuthenticateSession(Guid sessionId, Guid studentId, string code, DateTime dob)
    {
        var sessionDTO = await _mediator.Send(new GetSessionById { SessionId = sessionId, IncludeStudent = true });
        
        var authorizeResultDTO = new AuthorizeResultDTO
        {
            Message = "Could not authenticate student/session",
            Success = false,
            Status = nameof(AuthorizeResultStatusTypes.InvalidCredentials)
        };
        
        if (sessionDTO.StudentId != studentId || 
            sessionDTO.Code != code ||
            sessionDTO.Student?.Dob.Date != dob.Date)
        {
            return authorizeResultDTO;
        }

        var claims = _buildClaims(studentId, sessionDTO.PersonId, sessionId, Array.Empty<string>());
        var token = _buildToken(claims);
        
        authorizeResultDTO.Message = "Successfully signed in";
        authorizeResultDTO.Token = new JwtSecurityTokenHandler().WriteToken(token);
        authorizeResultDTO.Status = nameof(AuthorizeResultStatusTypes.Success);
        authorizeResultDTO.Expires = (int) (token.ValidTo - DateTime.UtcNow).TotalSeconds;
        authorizeResultDTO.Success = true;

        return authorizeResultDTO;
    }
    
    private static IEnumerable<Claim> _buildClaims(Guid studentId, Guid personId, Guid sessionId, IEnumerable<string> scopes)
    {
        var claims = new List<Claim>()
        {
            new(JwtRegisteredClaimNames.Sub, studentId.ToString()),
            new(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
            new(AppRegisteredClaimNames.Pid, personId.ToString()),
            new(ClaimTypes.Role, "Student"),
            new(AppRegisteredClaimNames.Scope, string.Join(" ", scopes)),
            new("sessionId", sessionId.ToString())
        };
        
        return claims;
    }
    
    private JwtSecurityToken _buildToken(IEnumerable<Claim> claims)
    {
        var issuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.SigningKey));

        return new JwtSecurityToken(
            _settings.Authority,
            _settings.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryInMinutes),
            notBefore: DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(-1)),
            signingCredentials: new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256Signature));
    }
}