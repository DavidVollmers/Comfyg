﻿using Comfyg.Authentication.Abstractions;
using Comfyg.Contracts.Requests;
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
    public IActionResult GetSettingValuesAsync([FromQuery] DateTimeOffset? since = null,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var values = since.HasValue
            ? GetValuesSinceAsync(clientIdentity, since.Value, cancellationToken)
            : GetValuesAsync(clientIdentity, cancellationToken);

        return Ok(values);
    }

    [HttpPost]
    public async Task<ActionResult> AddSettingValuesAsync([FromBody] AddSettingValuesRequest request,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var result = await AddValuesAsync(clientIdentity, request.Values, cancellationToken);

        if (!result) return Forbid();

        return Ok();
    }
}
