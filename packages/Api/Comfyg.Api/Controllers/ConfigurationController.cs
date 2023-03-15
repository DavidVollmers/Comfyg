using Comfyg.Authentication.Abstractions;
using Comfyg.Contracts.Configuration;
using Comfyg.Contracts.Requests;
using Comfyg.Contracts.Responses;
using Comfyg.Core.Abstractions.Changes;
using Comfyg.Core.Abstractions.Configuration;
using Comfyg.Core.Abstractions.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Api.Controllers;

[Authorize]
[ApiController]
[Route("configuration")]
public class ConfigurationController : ControllerBase
{
    private readonly IConfigurationService _configurationService;
    private readonly IPermissionService _permissionService;
    private readonly IChangeService _changeService;

    public ConfigurationController(IConfigurationService configurationService, IPermissionService permissionService,
        IChangeService changeService)
    {
        _configurationService = configurationService;
        _permissionService = permissionService;
        _changeService = changeService;
    }

    [HttpGet]
    public async Task<ActionResult<GetConfigurationValuesResponse>> GetConfigurationValuesAsync()
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var configurationValues = await _configurationService
            .GetConfigurationValuesAsync(clientIdentity.Client.ClientId).ConfigureAwait(false);

        return Ok(new GetConfigurationValuesResponse(configurationValues));
    }

    [HttpGet("fromDiff")]
    public async Task<ActionResult<GetConfigurationValuesResponse>> GetConfigurationValuesFromDiffAsync(
        [FromQuery] DateTime since)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var changes = await _changeService
            .GetChangesForOwnerAsync<IConfigurationValue>(clientIdentity.Client.ClientId, since.ToUniversalTime())
            .ConfigureAwait(false);

        var configurationValues = new List<IConfigurationValue>();
        foreach (var change in changes)
        {
            var configurationValue = await _configurationService.GetConfigurationValueAsync(change.TargetId)
                .ConfigureAwait(false);

            if (configurationValue == null) continue;

            configurationValues.Add(configurationValue);
        }

        return Ok(new GetConfigurationValuesResponse(configurationValues));
    }

    [HttpPost]
    public async Task<ActionResult> AddConfigurationValuesAsync([FromBody] AddConfigurationValuesRequest request)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        foreach (var configurationValue in request.ConfigurationValues)
        {
            var isPermitted = await _permissionService
                .IsPermittedAsync<IConfigurationValue>(clientIdentity.Client.ClientId, configurationValue.Key)
                .ConfigureAwait(false);
            if (!isPermitted) return Forbid();
        }

        foreach (var configurationValue in request.ConfigurationValues)
        {
            await _configurationService
                .AddConfigurationValueAsync(clientIdentity.Client.ClientId, configurationValue.Key,
                    configurationValue.Value)
                .ConfigureAwait(false);
        }

        return Ok();
    }
}