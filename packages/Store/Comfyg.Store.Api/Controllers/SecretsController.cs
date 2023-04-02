using Comfyg.Store.Api.Models;
using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Requests;
using Comfyg.Store.Core.Abstractions;
using Comfyg.Store.Core.Abstractions.Changes;
using Comfyg.Store.Core.Abstractions.Permissions;
using Comfyg.Store.Core.Abstractions.Secrets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Store.Api.Controllers;

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
    public IActionResult GetSecretValuesAsync([FromQuery] DateTimeOffset? since = null,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var values = since.HasValue
            ? GetValuesSinceAsync(clientIdentity, since.Value, cancellationToken)
            : GetValuesAsync(clientIdentity, cancellationToken);

        return Ok(values);
    }

    [HttpPost]
    public async Task<ActionResult> AddSecretValuesAsync([FromBody] IAddSecretValuesRequest request,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var result = await AddValuesAsync(clientIdentity, request.Values, cancellationToken);

        if (!result) return Forbid();

        return Ok();
    }

    protected override async Task<ISecretValue?> ConvertValueToAsync(ISecretValue value,
        CancellationToken cancellationToken)
    {
        var protectedValue = await _secretService.ProtectSecretValueAsync(value.Value, cancellationToken);

        return new SecretValueModel(value) { Value = protectedValue };
    }

    protected override async Task<ISecretValue?> ConvertValueFromAsync(ISecretValue value,
        CancellationToken cancellationToken)
    {
        var unprotectedValue = await _secretService.UnprotectSecretValueAsync(value.Value, cancellationToken);

        return new SecretValueModel(value) { Value = unprotectedValue };
    }
}
