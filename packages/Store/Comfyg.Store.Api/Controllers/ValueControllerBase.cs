using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Contracts;
using Comfyg.Store.Core.Abstractions;
using Comfyg.Store.Core.Abstractions.Changes;
using Comfyg.Store.Core.Abstractions.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Store.Api.Controllers;

public abstract class ValueControllerBase<T> : ControllerBase where T : IComfygValue
{
    private readonly IValueService<T> _valueService;
    private readonly IPermissionService _permissionService;
    private readonly IChangeService _changeService;

    protected ValueControllerBase(IValueService<T> valueService, IPermissionService permissionService,
        IChangeService changeService)
    {
        _valueService = valueService;
        _permissionService = permissionService;
        _changeService = changeService;
    }

    protected async Task<T?> TagValueAsync(IClientIdentity clientIdentity, string key, string version, string tag,
        CancellationToken cancellationToken)
    {
        var isPermitted = await _permissionService.IsPermittedAsync<T>(clientIdentity.Client.ClientId, key,
            Permissions.Write, cancellationToken: cancellationToken);
        if (!isPermitted) return default;

        return await _valueService.TagValueAsync(clientIdentity.Client.ClientId, key, tag, version, cancellationToken);
    }

    protected async Task<T?> GetValueAsync(IClientIdentity clientIdentity, string key, string? version,
        CancellationToken cancellationToken)
    {
        var isPermitted = await _permissionService.IsPermittedAsync<T>(clientIdentity.Client.ClientId, key,
            Permissions.Read, cancellationToken: cancellationToken);
        if (!isPermitted) return default;

        return await _valueService.GetValueAsync(key, version ?? ComfygConstants.LatestVersion, cancellationToken);
    }

    protected async IAsyncEnumerable<T> GetValuesAsync(IClientIdentity clientIdentity, string[]? tags,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var values = _valueService.GetLatestValuesAsync(clientIdentity.Client.ClientId, cancellationToken);

        await foreach (var value in values.WithCancellation(cancellationToken))
        {
            var result = await ResolveValueByTagsAsync(value, tags, cancellationToken);

            result = await ConvertValueFromAsync(result, cancellationToken);

            if (result == null) continue;

            yield return result;
        }
    }

    protected async IAsyncEnumerable<T> GetValuesSinceAsync(IClientIdentity clientIdentity,
        DateTimeOffset since, string[]? tags, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var changes =
            _changeService.GetChangesForOwnerAsync<T>(clientIdentity.Client.ClientId, since, cancellationToken);

        await foreach (var change in changes.GroupBy(c => c.TargetId).WithCancellation(cancellationToken))
        {
            var value = await _valueService.GetLatestValueAsync(change.Key, cancellationToken);

            if (value == null) continue;

            var result = await ResolveValueByTagsAsync(value, tags, cancellationToken);

            result = await ConvertValueFromAsync(result, cancellationToken);

            if (result == null) continue;

            yield return result;
        }
    }

    protected async Task<bool> AddValuesAsync(IClientIdentity clientIdentity, IEnumerable<T> values,
        CancellationToken cancellationToken)
    {
        var comfygValues = values as T[] ?? values.ToArray();

        foreach (var value in comfygValues)
        {
            if (string.IsNullOrWhiteSpace(value.Key)) throw new InvalidOperationException("Key cannot be empty.");

            var isPermitted = await _permissionService
                .IsPermittedAsync<T>(clientIdentity.Client.ClientId, value.Key, Permissions.Write, false,
                    cancellationToken);
            if (!isPermitted) return false;
        }

        foreach (var value in comfygValues)
        {
            var convertedValue = await ConvertValueToAsync(value, cancellationToken);

            if (convertedValue == null) continue;

            var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(value.Value)));

            await _valueService
                .AddValueAsync(clientIdentity.Client.ClientId, convertedValue.Key, convertedValue.Value,
                    hash, cancellationToken);
        }

        return true;
    }

    protected virtual Task<T?> ConvertValueToAsync(T value, CancellationToken cancellationToken)
    {
        return Task.FromResult(value)!;
    }

    protected virtual Task<T?> ConvertValueFromAsync(T value, CancellationToken cancellationToken)
    {
        return Task.FromResult(value)!;
    }

    private async Task<T> ResolveValueByTagsAsync(T value, IReadOnlyList<string>? tags,
        CancellationToken cancellationToken)
    {
        if (tags == null || tags.Count == 0) return value;

        for (var i = tags.Count - 1; i >= 0; i--)
        {
            var taggedValue =
                await _valueService.GetValueAsync(value.Key, $"{value.Version}-{tags[i]}", cancellationToken);

            if (taggedValue != null) return taggedValue;
        }

        return value;
    }
}
