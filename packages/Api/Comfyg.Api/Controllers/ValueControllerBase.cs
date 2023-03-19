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

    protected async Task<IEnumerable<T>> GetValuesAsync(IClientIdentity clientIdentity)
    {
        return await _valueService.GetValuesAsync(clientIdentity.Client.ClientId).ConfigureAwait(false);
    }

    protected async Task<IEnumerable<T>> GetValuesFromDiffAsync(IClientIdentity clientIdentity, DateTime since)
    {
        var changes = await _changeService
            .GetChangesForOwnerAsync<T>(clientIdentity.Client.ClientId, since.ToUniversalTime())
            .ConfigureAwait(false);

        var values = new List<T>();
        foreach (var change in changes)
        {
            var value = await _valueService.GetValueAsync(change.TargetId).ConfigureAwait(false);

            if (value == null) continue;

            values.Add(value);
        }

        return values;
    }

    protected async Task<bool> AddValuesAsync(IClientIdentity clientIdentity, IEnumerable<T> values)
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
            await _valueService
                .AddValueAsync(clientIdentity.Client.ClientId, value.Key, value.Value)
                .ConfigureAwait(false);
        }

        return true;
    }
}