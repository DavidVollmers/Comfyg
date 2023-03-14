using Comfyg.Authentication.Abstractions;
using Comfyg.Contracts.Configuration;
using Comfyg.Contracts.Responses;
using Comfyg.Core.Abstractions.Changes;
using Comfyg.Core.Abstractions.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Api.Controllers;

[Authorize]
[ApiController]
[Route("diff")]
public class DiffController : ControllerBase
{
    private readonly IPermissionService _permissionService;
    private readonly IChangeService _changeService;

    public DiffController(IPermissionService permissionService, IChangeService changeService)
    {
        _permissionService = permissionService;
        _changeService = changeService;
    }

    [HttpGet("configuration")]
    public async Task<ActionResult<GetDiffResponse>> GetConfigurationDiffAsync([FromQuery] DateTime since)
    {
        return await CalculateDiffAsync<IConfigurationValue>(since);
    }

    private async Task<ActionResult<GetDiffResponse>> CalculateDiffAsync<T>(DateTime since)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var permissions = await _permissionService.GetPermissionsAsync<T>(clientIdentity.Client.ClientId)
            .ConfigureAwait(false);

        var changes = await _changeService.GetChangesSinceAsync<T>(since).ConfigureAwait(false);

        var relevantChanges = changes.Where(c => permissions.Any(p => p.TargetId == c.TargetId));

        return Ok(new GetDiffResponse(relevantChanges));
    }
}