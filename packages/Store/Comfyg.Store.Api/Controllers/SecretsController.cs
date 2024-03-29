﻿using Comfyg.Store.Api.Models;
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
    public IActionResult GetSecretValues([FromQuery] DateTimeOffset? since = null,
        [FromQuery] string[]? tags = null, CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var values = since.HasValue
            ? GetValuesSinceAsync(clientIdentity, since.Value, tags, cancellationToken)
            : GetValuesAsync(clientIdentity, tags, cancellationToken);

        return Ok(values);
    }

    [HttpGet("{key}/{version?}")]
    public async Task<IActionResult> GetSecretValueAsync([FromRoute] string key, [FromRoute] string version,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var value = await GetValueAsync(clientIdentity, key, version, cancellationToken);

        if (value == null) return Forbid();

        return Ok(value);
    }

    [HttpPost]
    public async Task<IActionResult> AddSecretValuesAsync([FromBody] IAddSecretValuesRequest request,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var result = await AddValuesAsync(clientIdentity, request.Values, cancellationToken);

        if (!result) return Forbid();

        return Ok();
    }

    [HttpPost("tags")]
    public async Task<IActionResult> TagSecretValueAsync([FromBody] ITagValueRequest request,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var result = await TagValueAsync(clientIdentity, request.Key, request.Version, request.Tag, cancellationToken);

        if (result == null) return Forbid();

        return Ok(result);
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
