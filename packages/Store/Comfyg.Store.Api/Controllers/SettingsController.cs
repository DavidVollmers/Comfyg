using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Requests;
using Comfyg.Store.Core.Abstractions;
using Comfyg.Store.Core.Abstractions.Changes;
using Comfyg.Store.Core.Abstractions.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Store.Api.Controllers;

[Authorize]
[ApiController]
[Route("settings")]
public class SettingsController : ValueControllerBase<ISettingValue>
{
    public SettingsController(IValueService<ISettingValue> valueService, IPermissionService permissionService,
        IChangeService changeService)
        : base(valueService, permissionService, changeService)
    {
    }

    [HttpGet]
    public IActionResult GetSettingValues([FromQuery] DateTimeOffset? since = null,
        [FromQuery] string[]? tags = null, CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var values = since.HasValue
            ? GetValuesSinceAsync(clientIdentity, since.Value, tags, cancellationToken)
            : GetValuesAsync(clientIdentity, tags, cancellationToken);

        return Ok(values);
    }

    [HttpGet("{key}/{version?}")]
    public async Task<IActionResult> GetSettingValueAsync([FromRoute] string key, [FromRoute] string version,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var value = await GetValueAsync(clientIdentity, key, version, cancellationToken);

        if (value == null) return Forbid();

        return Ok(value);
    }

    [HttpPost]
    public async Task<IActionResult> AddSettingValuesAsync([FromBody] IAddSettingValuesRequest request,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var result = await AddValuesAsync(clientIdentity, request.Values, cancellationToken);

        if (!result) return Forbid();

        return Ok();
    }

    [HttpPost("tags")]
    public async Task<IActionResult> TagSettingValueAsync([FromBody] ITagValueRequest request,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var result = await TagValueAsync(clientIdentity, request.Key, request.Version, request.Tag, cancellationToken);

        if (result == null) return Forbid();

        return Ok(result);
    }
}
