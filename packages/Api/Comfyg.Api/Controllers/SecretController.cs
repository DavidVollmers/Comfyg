using Comfyg.Authentication.Abstractions;
using Comfyg.Contracts.Requests;
using Comfyg.Contracts.Responses;
using Comfyg.Contracts.Secrets;
using Comfyg.Core.Abstractions.Changes;
using Comfyg.Core.Abstractions.Permissions;
using Comfyg.Core.Abstractions.Secrets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Api.Controllers;

[Authorize]
[ApiController]
[Route("secrets")]
public class SecretController : ControllerBase
{
    private readonly ISecretService _secretService;
    private readonly IPermissionService _permissionService;
    private readonly IChangeService _changeService;

    public SecretController(ISecretService secretService, IPermissionService permissionService,
        IChangeService changeService)
    {
        _secretService = secretService;
        _permissionService = permissionService;
        _changeService = changeService;
    }

    [HttpGet]
    public async Task<ActionResult<GetSecretValuesResponse>> GetSecretsAsync()
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var secretValues =
            await _secretService.GetSecretValuesAsync(clientIdentity.Client.ClientId).ConfigureAwait(false);

        return Ok(new GetSecretValuesResponse(secretValues));
    }

    [HttpGet("fromDiff")]
    public async Task<ActionResult<GetSecretValuesResponse>> GetSecretsFromDiffAsync([FromQuery] DateTime since)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var changes = await _changeService
            .GetChangesForOwnerAsync<ISecretValue>(clientIdentity.Client.ClientId, since.ToUniversalTime())
            .ConfigureAwait(false);

        var secretValues = new List<ISecretValue>();
        foreach (var change in changes)
        {
            var secretValue = await _secretService.GetSecretValueAsync(change.TargetId).ConfigureAwait(false);

            if (secretValue == null) continue;

            secretValues.Add(secretValue);
        }

        return Ok(new GetSecretValuesResponse(secretValues));
    }

    [HttpPost]
    public async Task<ActionResult> AddSecretValuesAsync([FromBody] AddSecretValuesRequest request)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        foreach (var secretValue in request.SecretValues)
        {
            var isPermitted = await _permissionService
                .IsPermittedAsync<ISecretValue>(clientIdentity.Client.ClientId, secretValue.Key)
                .ConfigureAwait(false);
            if (!isPermitted) return Forbid();
        }

        foreach (var secretValue in request.SecretValues)
        {
            await _secretService
                .AddSecretValueAsync(clientIdentity.Client.ClientId, secretValue.Key, secretValue.Value)
                .ConfigureAwait(false);
        }

        return Ok();
    }
}