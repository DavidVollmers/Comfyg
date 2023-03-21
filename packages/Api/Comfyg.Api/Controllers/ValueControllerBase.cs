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

    protected async Task<IEnumerable<T>> GetValuesAsync(IClientIdentity clientIdentity,
        CancellationToken cancellationToken)
    {
        var values = await _valueService.GetValuesAsync(clientIdentity.Client.ClientId).ConfigureAwait(false);

        var convertedValues = new List<T>();
        foreach (var value in values)
        {
            var convertedValue = await ConvertValueFromAsync(value, cancellationToken).ConfigureAwait(false);

            if (convertedValue == null) continue;

            convertedValues.Add(convertedValue);
        }

        return convertedValues;
    }

    protected async Task<IEnumerable<T>> GetValuesFromDiffAsync(IClientIdentity clientIdentity, DateTime since,
        CancellationToken cancellationToken)
    {
        var changes = await _changeService
            .GetChangesForOwnerAsync<T>(clientIdentity.Client.ClientId, since)
            .ConfigureAwait(false);

        var values = new List<T>();
        foreach (var change in changes)
        {
            var value = await _valueService.GetValueAsync(change.TargetId).ConfigureAwait(false);

            if (value == null) continue;

            var convertedValue = await ConvertValueFromAsync(value, cancellationToken).ConfigureAwait(false);

            if (convertedValue == null) continue;

            values.Add(convertedValue);
        }

        return values;
    }

    protected async Task<bool> AddValuesAsync(IClientIdentity clientIdentity, IEnumerable<T> values,
        CancellationToken cancellationToken)
    {
        var comfygValues = values as T[] ?? values.ToArray();

        foreach (var value in comfygValues)
        {
            var isPermitted = await _permissionService
                .IsPermittedAsync<T>(clientIdentity.Client.ClientId, value.Key)
                .ConfigureAwait(false);
            if (!isPermitted) return false;
        }

        foreach (var value in comfygValues)
        {
            var convertedValue = await ConvertValueToAsync(value, cancellationToken).ConfigureAwait(false);

            if (convertedValue == null) continue;

            await _valueService
                .AddValueAsync(clientIdentity.Client.ClientId, convertedValue.Key, convertedValue.Value)
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