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
    public async Task<ActionResult<GetConfigurationValuesResponse>> GetConfigurationValuesAsync()
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var configurationValues = await GetValuesAsync(clientIdentity).ConfigureAwait(false);

        return Ok(new GetConfigurationValuesResponse(configurationValues));
    }

    [HttpGet("fromDiff")]
    public async Task<ActionResult<GetConfigurationValuesResponse>> GetConfigurationValuesFromDiffAsync(
        [FromQuery] DateTime since)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var configurationValues = await GetValuesFromDiffAsync(clientIdentity, since).ConfigureAwait(false);

        return Ok(new GetConfigurationValuesResponse(configurationValues));
    }

    [HttpPost]
    public async Task<ActionResult> AddConfigurationValuesAsync([FromBody] AddConfigurationValuesRequest request)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var result = await AddValuesAsync(clientIdentity, request.Values).ConfigureAwait(false);

        if (!result) return Forbid();

        return Ok();
    }
}