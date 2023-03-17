using Comfyg.Authentication.Abstractions;
using Comfyg.Contracts.Requests;
using Comfyg.Contracts.Responses;
using Comfyg.Contracts.Settings;
using Comfyg.Core.Abstractions.Changes;
using Comfyg.Core.Abstractions.Permissions;
using Comfyg.Core.Abstractions.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Api.Controllers;

[Authorize]
[ApiController]
[Route("settings")]
public class SettingsController : ControllerBase
{
    private readonly ISettingService _settingService;
    private readonly IPermissionService _permissionService;
    private readonly IChangeService _changeService;

    public SettingsController(ISettingService settingService, IPermissionService permissionService,
        IChangeService changeService)
    {
        _settingService = settingService;
        _permissionService = permissionService;
        _changeService = changeService;
    }

    [HttpGet]
    public async Task<ActionResult<GetSettingValuesResponse>> GetSettingValuesAsync()
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var settingValues =
            await _settingService.GetSettingValuesAsync(clientIdentity.Client.ClientId).ConfigureAwait(false);

        return Ok(new GetSettingValuesResponse(settingValues));
    }

    [HttpGet("fromDiff")]
    public async Task<ActionResult<GetSettingValuesResponse>> GetSettingValuesFromDiffAsync([FromQuery] DateTime since)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var changes = await _changeService
            .GetChangesForOwnerAsync<ISettingValue>(clientIdentity.Client.ClientId, since.ToUniversalTime())
            .ConfigureAwait(false);

        var settingValues = new List<ISettingValue>();
        foreach (var change in changes)
        {
            var settingValue = await _settingService.GetSettingValueAsync(change.TargetId).ConfigureAwait(false);

            if (settingValue == null) continue;

            settingValues.Add(settingValue);
        }

        return Ok(new GetSettingValuesResponse(settingValues));
    }

    [HttpPost]
    public async Task<ActionResult> AddSettingValuesAsync([FromBody] AddSettingValuesRequest request)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        foreach (var settingValue in request.SettingValues)
        {
            var isPermitted = await _permissionService
                .IsPermittedAsync<ISettingValue>(clientIdentity.Client.ClientId, settingValue.Key)
                .ConfigureAwait(false);
            if (!isPermitted) return Forbid();
        }

        foreach (var settingValue in request.SettingValues)
        {
            await _settingService
                .AddSettingValueAsync(clientIdentity.Client.ClientId, settingValue.Key, settingValue.Value)
                .ConfigureAwait(false);
        }

        return Ok();
    }
}