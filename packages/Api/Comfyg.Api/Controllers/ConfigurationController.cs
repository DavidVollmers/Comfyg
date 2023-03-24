using Comfyg.Authentication.Abstractions;
using Comfyg.Contracts.Configuration;
using Comfyg.Contracts.Requests;
using Comfyg.Contracts.Responses;
using Comfyg.Core.Abstractions;
using Comfyg.Core.Abstractions.Changes;
using Comfyg.Core.Abstractions.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Api.Controllers;

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
    public async Task<ActionResult<GetConfigurationValuesResponse>> GetConfigurationValuesAsync(
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        //TODO support streaming
        var values = await GetValuesAsync(clientIdentity, cancellationToken).ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        return Ok(new GetConfigurationValuesResponse(values));
    }

    [HttpGet("fromDiff")]
    public async Task<ActionResult<GetConfigurationValuesResponse>> GetConfigurationValuesFromDiffAsync(
        [FromQuery] DateTime since, CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        //TODO support streaming
        var values = await GetValuesFromDiffAsync(clientIdentity, since, cancellationToken)
            .ToArrayAsync(cancellationToken).ConfigureAwait(false);

        return Ok(new GetConfigurationValuesResponse(values));
    }

    [HttpPost]
    public async Task<ActionResult> AddConfigurationValuesAsync([FromBody] AddConfigurationValuesRequest request,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var result = await AddValuesAsync(clientIdentity, request.Values, cancellationToken).ConfigureAwait(false);

        if (!result) return Forbid();

        return Ok();
    }
}
