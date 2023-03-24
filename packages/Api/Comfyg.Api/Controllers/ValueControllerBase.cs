using System.Runtime.CompilerServices;
using Comfyg.Authentication.Abstractions;
using Comfyg.Contracts;
using Comfyg.Core.Abstractions;
using Comfyg.Core.Abstractions.Changes;
using Comfyg.Core.Abstractions.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Api.Controllers;

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

    protected async IAsyncEnumerable<IComfygValue> GetValuesAsync(IClientIdentity clientIdentity,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var values = _valueService.GetValuesAsync(clientIdentity.Client.ClientId, cancellationToken);

        await foreach (var value in values.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            var convertedValue = await ConvertValueFromAsync(value, cancellationToken).ConfigureAwait(false);

            if (convertedValue == null) continue;

            yield return convertedValue;
        }
    }

    protected async IAsyncEnumerable<IComfygValue> GetValuesSinceAsync(IClientIdentity clientIdentity,
        DateTimeOffset since, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var changes =
            _changeService.GetChangesForOwnerAsync<T>(clientIdentity.Client.ClientId, since, cancellationToken);

        await foreach (var change in changes.GroupBy(c => c.TargetId).WithCancellation(cancellationToken)
                           .ConfigureAwait(false))
        {
            var value = await _valueService.GetLatestValueAsync(change.Key, cancellationToken)
                .ConfigureAwait(false);

            if (value == null) continue;

            var convertedValue = await ConvertValueFromAsync(value, cancellationToken).ConfigureAwait(false);

            if (convertedValue == null) continue;

            yield return convertedValue;
        }
    }

    protected async Task<bool> AddValuesAsync(IClientIdentity clientIdentity, IEnumerable<T> values,
        CancellationToken cancellationToken)
    {
        var comfygValues = values as T[] ?? values.ToArray();

        foreach (var value in comfygValues)
        {
            var isPermitted = await _permissionService
                .IsPermittedAsync<T>(clientIdentity.Client.ClientId, value.Key, cancellationToken)
                .ConfigureAwait(false);
            if (!isPermitted) return false;
        }

        foreach (var value in comfygValues)
        {
            var convertedValue = await ConvertValueToAsync(value, cancellationToken).ConfigureAwait(false);

            if (convertedValue == null) continue;

            await _valueService
                .AddValueAsync(clientIdentity.Client.ClientId, convertedValue.Key, convertedValue.Value,
                    cancellationToken)
                .ConfigureAwait(false);
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
}
