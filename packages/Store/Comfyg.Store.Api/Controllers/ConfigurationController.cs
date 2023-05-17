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
[Route("configuration")]
public class ConfigurationController : ValueControllerBase<IConfigurationValue>
{
    public ConfigurationController(IValueService<IConfigurationValue> valueService,
        IPermissionService permissionService, IChangeService changeService)
        : base(valueService, permissionService, changeService)
    {
    }

    /// <summary>
    /// Get Comfyg configuration values.
    /// </summary>
    /// <param name="since">Optional <see cref="DateTimeOffset"/> to only get Comfyg configuration values which were added or updated since then.</param>
    /// <param name="tags">Optional array of tags to filter the Comfyg configuration values.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> controlling the request lifespan.</param>
    /// <returns><see cref="IAsyncEnumerable{IConfigurationValue}"/></returns>
    [HttpGet]
    public IActionResult GetConfigurationValues([FromQuery] DateTimeOffset? since = null,
        [FromQuery] string[]? tags = null, CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var values = since.HasValue
            ? GetValuesSinceAsync(clientIdentity, since.Value, tags, cancellationToken)
            : GetValuesAsync(clientIdentity, tags, cancellationToken);

        return Ok(values);
    }

    [HttpGet("{key}/{version?}")]
    public async Task<IActionResult> GetConfigurationValueAsync([FromRoute] string key, [FromRoute] string version,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var value = await GetValueAsync(clientIdentity, key, version, cancellationToken);

        if (value == null) return Forbid();

        return Ok(value);
    }

    [HttpPost]
    public async Task<IActionResult> AddConfigurationValuesAsync(
        [FromBody] IAddConfigurationValuesRequest request, CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var result = await AddValuesAsync(clientIdentity, request.Values, cancellationToken);

        if (!result) return Forbid();

        return Ok();
    }

    [HttpPost("tags")]
    public async Task<IActionResult> TagConfigurationValueAsync([FromBody] ITagValueRequest request,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var result = await TagValueAsync(clientIdentity, request.Key, request.Version, request.Tag, cancellationToken);

        if (result == null) return Forbid();

        return Ok(result);
    }
}
