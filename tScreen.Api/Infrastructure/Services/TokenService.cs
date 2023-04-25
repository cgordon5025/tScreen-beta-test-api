using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common;
using Application.Common.Interfaces;
using Core.Settings.Models;
using Domain.Entities.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;

    public TokenService(JwtSettings settings)
    {
        _settings = settings;
    }

    public IEnumerable<Claim> BuildClaims(User user, Guid personId, Guid locationId, 
        IEnumerable<string> roles, IEnumerable<string> scopes)
    {
        var claims = new List<Claim>()
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Email),
            new(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
            new(AppRegisteredClaimNames.Pid, personId.ToString()),
            new(AppRegisteredClaimNames.Cid, user.CompanyId.ToString()),
            new(AppRegisteredClaimNames.Lid, locationId.ToString()),
            new(AppRegisteredClaimNames.Roles, string.Join(" ", roles)),
            new(AppRegisteredClaimNames.Scope, string.Join(" ", scopes))
        };
        
        return claims;
    }

    public JwtSecurityToken BuildToken(IEnumerable<Claim> claims)
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