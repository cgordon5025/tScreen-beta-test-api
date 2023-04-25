using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Application.Common;
using HotChocolate;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace GraphQl.GraphQl.Attributes;

public class CurrentPersonContextAttribute : GlobalStateAttribute
{
    public CurrentPersonContextAttribute() : base(nameof(CurrentPersonContext)) { }
}

public class CurrentPersonContext
{
    public Guid UserId { get; private set; }
    public Guid PersonId { get; private set; }
    public Guid LocationId { get; private set;}
    public Guid CompanyId { get; private set; }

    public IEnumerable<Claim> Claims { get; private set; } = null!;
    public IEnumerable<string>? Roles { get; private set; }
    public IEnumerable<string>? Scopes { get; private set; }

    public CurrentPersonContext(ClaimsPrincipal usePrincipal, Guid companyId, Guid locationId)
    {
        var cvPersonId = usePrincipal.FindFirstValue(AppRegisteredClaimNames.Pid);
        var cvCompanyId = usePrincipal.FindFirstValue(AppRegisteredClaimNames.Cid);
        var cvLocationId = usePrincipal.FindFirstValue(AppRegisteredClaimNames.Lid);
        
        var claimUserId = Guid.Parse(usePrincipal.FindFirstValue(JwtRegisteredClaimNames.Sub));
        var claimPersonId = !string.IsNullOrWhiteSpace(cvPersonId) ? Guid.Parse(cvPersonId) : Guid.Empty;
        var claimCompanyId = !string.IsNullOrWhiteSpace(cvCompanyId) ? Guid.Parse(cvCompanyId) : companyId;
        var claimLocationId = !string.IsNullOrWhiteSpace(cvLocationId) ? Guid.Parse(cvLocationId) : locationId;
        
        _initializeCurrentPerson(claimUserId, claimPersonId, claimCompanyId, claimLocationId, usePrincipal.Claims);
    }
    
    public CurrentPersonContext(Guid userId, Guid personId, Guid companyId, Guid locationId, IEnumerable<Claim> claims)
    {
        _initializeCurrentPerson(userId, personId, companyId, locationId, claims);
    }

    private void _initializeCurrentPerson(Guid userId, Guid personId, Guid companyId, Guid locationId,
        IEnumerable<Claim> claims)
    {
        if (personId == Guid.Empty)
            throw new Exception($"{nameof(personId)} must be a non-empty Guid");
        
        if (companyId == Guid.Empty)
            throw new Exception($"{nameof(companyId)} must be a non-empty Guid");
        
        if (locationId == Guid.Empty)
            throw new Exception($"{nameof(locationId)} must be a non-empty Guid");

        UserId = userId;
        PersonId = personId;
        CompanyId = companyId;
        LocationId = locationId;

        var claimsList = claims.ToList();

        if (!claimsList.Any()) 
            return;

        var allowedRoles = claimsList
            .FirstOrDefault(x => x.Type == AppRegisteredClaimNames.Roles)
            ?.Value;

        var allowedScopes = claimsList
            .FirstOrDefault(e => e.Type == AppRegisteredClaimNames.Scope)
            ?.Value;

        Roles = allowedRoles?.Split(" ");
        Scopes = allowedScopes?.Split(" ");
        Claims = claimsList;
    }
}