using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using GraphQl.GraphQl.Features.Objects.Me.Inputs;
using GraphQl.GraphQl.Models;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Features.Objects.Me;

[ExtendObjectType("Mutation")]
public class MeMutation
{
    [HotChocolate.AspNetCore.Authorization.Authorize(Policy = "AllowAnonymous")]
    public async Task<Models.AuthenticationResult> Authenticate(
        AuthenticateInput input,
        [Service] IIdentityService identityService)
    {
        var (email, password) = input;
        var result = await identityService.Authenticate(email, password);
        return new AuthenticationResult
        {
            Message = result.Message,
            AccessToken = result.Token,
            Expires = Math.Round(result.Expires).ToString(CultureInfo.InvariantCulture),
            Status = result.Status,
            Success = result.Success
        };
    }

    [HotChocolate.AspNetCore.Authorization.Authorize]
    public async Task<bool> VerifyPassword(
        VerifyPasswordInput input,
        [Service] IIdentityService identityService)
    {
        var (email, password) = input;
        var result = await identityService.ConfirmPassword(email, password);
        return result;
    }
}