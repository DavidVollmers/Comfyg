using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Requests;
using Comfyg.Store.Core.Abstractions.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Store.Api.Controllers;

[Authorize]
[ApiController]
[Route("permissions")]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionsController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpPost("config")]
    public async Task<IActionResult> SetConfigurationPermissionsAsync([FromBody] ISetPermissionRequest[] request,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var result = await SetPermissionsAsync<IConfigurationValue>(clientIdentity, request, cancellationToken);

        return result ? Ok() : Forbid();
    }

    [HttpPost("secrets")]
    public async Task<IActionResult> SetSecretPermissionsAsync([FromBody] ISetPermissionRequest[] request,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var result = await SetPermissionsAsync<ISecretValue>(clientIdentity, request, cancellationToken);

        return result ? Ok() : Forbid();
    }

    [HttpPost("settings")]
    public async Task<IActionResult> SetSettingPermissionsAsync([FromBody] ISetPermissionRequest[] request,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var result = await SetPermissionsAsync<ISettingValue>(clientIdentity, request, cancellationToken);

        return result ? Ok() : Forbid();
    }

    private async Task<bool> SetPermissionsAsync<T>(IClientIdentity clientIdentity, ISetPermissionRequest[] permissions,
        CancellationToken cancellationToken) where T : IComfygValue
    {
        foreach (var permission in permissions)
        {
            var isPermitted = await _permissionService
                .IsPermittedAsync<T>(clientIdentity.Client.ClientId, permission.Key,
                    cancellationToken: cancellationToken);
            if (!isPermitted) return false;
        }

        foreach (var permission in permissions)
        {
            await _permissionService.SetPermissionAsync<T>(clientIdentity.Client.ClientId, permission.Key,
                cancellationToken);
        }

        return true;
    }
}
