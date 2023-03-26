using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Contracts.Configuration;
using Comfyg.Store.Contracts.Requests;
using Comfyg.Store.Core.Abstractions;
using Comfyg.Store.Core.Abstractions.Changes;
using Comfyg.Store.Core.Abstractions.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Store.Api.Controllers;

[Authorize]
[ApiController]
[Route("configuration")]
public class ConfigurationController : ValueControllerBase<IConfigurationValue>
{
    public ConfigurationController(IValueService<IConfigurationValue> valueService,
        IPermissionService permissionService, IChangeService changeService)
        : base(valueService, permissionService, changeService)
    {
    }

    [HttpGet]
    public IActionResult GetConfigurationValuesAsync([FromQuery] DateTimeOffset? since = null,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var values = since.HasValue
            ? GetValuesSinceAsync(clientIdentity, since.Value, cancellationToken)
            : GetValuesAsync(clientIdentity, cancellationToken);

        return Ok(values);
    }

    [HttpPost]
    public async Task<ActionResult> AddConfigurationValuesAsync([FromBody] AddConfigurationValuesRequest request,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var result = await AddValuesAsync(clientIdentity, request.Values, cancellationToken);

        if (!result) return Forbid();

        return Ok();
    }
}
