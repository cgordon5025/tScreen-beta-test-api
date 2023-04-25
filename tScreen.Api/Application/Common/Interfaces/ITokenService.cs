using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Domain.Entities.Identity;

namespace Application.Common.Interfaces;

public interface ITokenService
{
    public IEnumerable<Claim> BuildClaims(User user, Guid personId, Guid locationId, 
        IEnumerable<string> roles, IEnumerable<string> scopes);
    public JwtSecurityToken BuildToken(IEnumerable<Claim> claims);
}