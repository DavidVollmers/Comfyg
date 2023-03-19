using Comfyg.Api.Models;
using Comfyg.Authentication.Abstractions;
using Comfyg.Contracts.Requests;
using Comfyg.Contracts.Responses;
using Comfyg.Contracts.Secrets;
using Comfyg.Core.Abstractions;
using Comfyg.Core.Abstractions.Changes;
using Comfyg.Core.Abstractions.Permissions;
using Comfyg.Core.Abstractions.Secrets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Api.Controllers;

[Authorize]
[ApiController]
[Route("secrets")]
public class SecretsController : ValueControllerBase<ISecretValue>
{
    private readonly ISecretService _secretService;

    public SecretsController(IValueService<ISecretValue> valueService, IPermissionService permissionService,
        IChangeService changeService, ISecretService secretService)
        : base(valueService, permissionService, changeService)
    {
        _secretService = secretService;
    }

    [HttpGet]
    public async Task<ActionResult<GetSecretValuesResponse>> GetSecretValuesAsync(
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var values = await GetValuesAsync(clientIdentity, cancellationToken).ConfigureAwait(false);

        return Ok(new GetSecretValuesResponse(values));
    }

    [HttpGet("fromDiff")]
    public async Task<ActionResult<GetSecretValuesResponse>> GetSecretValuesFromDiffAsync([FromQuery] DateTime since,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var values = await GetValuesFromDiffAsync(clientIdentity, since, cancellationToken).ConfigureAwait(false);

        return Ok(new GetSecretValuesResponse(values));
    }

    [HttpPost]
    public async Task<ActionResult> AddSecretValuesAsync([FromBody] AddSecretValuesRequest request,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var result = await AddValuesAsync(clientIdentity, request.Values, cancellationToken).ConfigureAwait(false);

        if (!result) return Forbid();

        return Ok();
    }

    protected override async Task<ISecretValue?> ConvertValueToAsync(ISecretValue value,
        CancellationToken cancellationToken)
    {
        var protectedValue = await _secretService.ProtectSecretValueAsync(value.Value, cancellationToken)
            .ConfigureAwait(false);

        return new SecretValueModel(value)
        {
            Value = protectedValue
        };
    }

    protected override async Task<ISecretValue?> ConvertValueFromAsync(ISecretValue value,
        CancellationToken cancellationToken)
    {
        var unprotectedValue = await _secretService.UnprotectSecretValueAsync(value.Value, cancellationToken)
            .ConfigureAwait(false);

        return new SecretValueModel(value)
        {
            Value = unprotectedValue
        };
    }
}