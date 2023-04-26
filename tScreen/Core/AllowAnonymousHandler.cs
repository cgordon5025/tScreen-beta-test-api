using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Core;

public class AllowAnonymousHandler : AuthorizationHandler<AllowAnonymousHandler>, IAuthorizationRequirement
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AllowAnonymousHandler handler)
    {
        context.Succeed(handler);
        return Task.CompletedTask;
    }
}