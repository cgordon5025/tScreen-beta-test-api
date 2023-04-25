using System;
using Microsoft.AspNetCore.Mvc;

namespace GraphQl.Controllers;

public abstract class BaseController : Controller
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public Guid GetCompanyIdHeaderId()
    {
        Request.Headers.TryGetValue("CID", out var companyId);
        return string.IsNullOrWhiteSpace(companyId) ? Guid.Empty : Guid.Parse(companyId);
    }
    
    [ApiExplorerSettings(IgnoreApi = true)]
    public Guid GetLocationIdFromHeader()
    {
        Request.Headers.TryGetValue("LID", out var locationId);
        return string.IsNullOrWhiteSpace(locationId) ? Guid.Empty : Guid.Parse(locationId);
    }
}