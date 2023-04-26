using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Core;
using GraphQl.GraphQl.Attributes;
using HotChocolate.AspNetCore;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Http;

namespace GraphQl.GraphQl.Interceptors;

public class HttpRequestInterceptor : DefaultHttpRequestInterceptor
{
    public override ValueTask OnCreateAsync(
        HttpContext context,
        IRequestExecutor requestExecutor,
        IQueryRequestBuilder requestBuilder,
        CancellationToken cancellationToken)
    {
        context.Request.Headers.TryGetValue(CompanyIdAttribute.Name, out var hvCompanyId);
        context.Request.Headers.TryGetValue(LocationIdAttribute.Name, out var hvLocationId);

        var companyId = !string.IsNullOrWhiteSpace(hvCompanyId)
            ? Guid.Parse(hvCompanyId)
            : Guid.Empty;

        var locationId = !string.IsNullOrWhiteSpace(hvLocationId)
            ? Guid.Parse(hvLocationId)
            : Guid.Empty;

        requestBuilder.SetProperty(CompanyIdAttribute.Name, companyId);
        requestBuilder.SetProperty(LocationIdAttribute.Name, locationId);
        requestBuilder.SetProperty(ReferenceCodeAttribute.Name, Utility.GetShortGuid());

        var sessionId = context.User.FindFirstValue("sessionId");

        if (sessionId is null && context.User.Identity is not null
            && context.User.Identity.IsAuthenticated)
        {
            var currentPersonContext = new CurrentPersonContext(context.User, companyId, locationId);
            requestBuilder.SetProperty(nameof(CurrentPersonContext), currentPersonContext);
        }

        return base.OnCreateAsync(context, requestExecutor, requestBuilder, cancellationToken);
    }
}