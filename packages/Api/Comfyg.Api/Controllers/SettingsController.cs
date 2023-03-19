using Comfyg.Authentication.Abstractions;
using Comfyg.Contracts.Requests;
using Comfyg.Contracts.Responses;
using Comfyg.Contracts.Settings;
using Comfyg.Core.Abstractions;
using Comfyg.Core.Abstractions.Changes;
using Comfyg.Core.Abstractions.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Api.Controllers;

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
    public async Task<ActionResult<GetSettingValuesResponse>> GetSettingValuesAsync(
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var values = await GetValuesAsync(clientIdentity, cancellationToken).ConfigureAwait(false);

        return Ok(new GetSettingValuesResponse(values));
    }

    [HttpGet("fromDiff")]
    public async Task<ActionResult<GetSettingValuesResponse>> GetSettingValuesFromDiffAsync([FromQuery] DateTime since,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var values = await GetValuesFromDiffAsync(clientIdentity, since, cancellationToken).ConfigureAwait(false);

        return Ok(new GetSettingValuesResponse(values));
    }

    [HttpPost]
    public async Task<ActionResult> AddSettingValuesAsync([FromBody] AddSettingValuesRequest request,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var result = await AddValuesAsync(clientIdentity, request.Values, cancellationToken).ConfigureAwait(false);

        if (!result) return Forbid();

        return Ok();
    }
}