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
    private readonly IClientService _clientService;

    public PermissionsController(IPermissionService permissionService, IClientService clientService)
    {
        _permissionService = permissionService;
        _clientService = clientService;
    }

    [HttpPost]
    public async Task<IActionResult> SetPermissionsAsync([FromBody] ISetPermissionsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var client = await _clientService.GetClientAsync(request.ClientId, cancellationToken);

        if (client == null) return Forbid();

        await SetPermissionsAsync<IConfigurationValue>(clientIdentity, client, cancellationToken);
        await SetPermissionsAsync<ISecretValue>(clientIdentity, client, cancellationToken);
        await SetPermissionsAsync<ISettingValue>(clientIdentity, client, cancellationToken);

        return Ok();
    }

    [HttpPost("configuration")]
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
        var clients = new List<string>();
        foreach (var permission in permissions)
        {
            var isPermitted = await _permissionService
                .IsPermittedAsync<T>(clientIdentity.Client.ClientId, permission.Key, Permissions.Permit,
                    cancellationToken: cancellationToken);
            if (!isPermitted) return false;

            if (clients.Contains(permission.ClientId)) continue;

            var client = await _clientService.GetClientAsync(permission.ClientId, cancellationToken);
            if (client == null) return false;

            clients.Add(client.ClientId);
        }

        foreach (var permission in permissions)
        {
            //TODO write/delete/permit option
            await _permissionService.SetPermissionAsync<T>(permission.ClientId, permission.Key, Permissions.Read,
                cancellationToken);
        }

        return true;
    }

    private async Task SetPermissionsAsync<T>(IClientIdentity clientIdentity, IClient client,
        CancellationToken cancellationToken) where T : IComfygValue
    {
        var permissions =
            _permissionService.GetPermissionsAsync<T>(clientIdentity.Client.ClientId, Permissions.Permit,
                cancellationToken);
        await foreach (var permission in permissions.WithCancellation(cancellationToken))
        {
            //TODO write/delete/permit option
            await _permissionService.SetPermissionAsync<T>(client.ClientId, permission.TargetId, Permissions.Read,
                cancellationToken);
        }
    }
}
