﻿using System.Runtime.CompilerServices;
using Azure.Data.Tables;
using Azure.Data.Tables.Poco;
using Comfyg.Contracts;
using Comfyg.Contracts.Changes;
using Comfyg.Core.Abstractions;
using Comfyg.Core.Abstractions.Changes;
using Comfyg.Core.Abstractions.Permissions;

namespace Comfyg.Core;

internal class ValueService<TValue, TEntity> : IValueService<TValue>
    where TValue : IComfygValue
    where TEntity : class, TValue, ISerializableComfygValue, new()
{
    private readonly TypedTableClient<TEntity> _values;
    private readonly IPermissionService _permissionService;
    private readonly IChangeService _changeService;

    public ValueService(string systemId, TableServiceClient tableServiceClient, IPermissionService permissionService,
        IChangeService changeService)
    {
        if (systemId == null) throw new ArgumentNullException(nameof(systemId));
        if (tableServiceClient == null) throw new ArgumentNullException(nameof(tableServiceClient));

        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
        _changeService = changeService ?? throw new ArgumentNullException(nameof(changeService));

        _values = tableServiceClient.GetTableClient<TEntity>().OverrideTableName($"{systemId}{typeof(TEntity).Name}");
    }

    public async Task AddValueAsync(string owner, string key, string value,
        CancellationToken cancellationToken = default)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (value == null) throw new ArgumentNullException(nameof(value));

        await _values.CreateTableIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

        foreach (var version in new[]
                     { CoreConstants.LatestVersion, (long.MaxValue - DateTimeOffset.UtcNow.Ticks).ToString() })
        {
            await _values.UpsertAsync(new TEntity
            {
                Key = key,
                Value = value,
                Version = version
            }, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        await _changeService.LogChangeAsync<TValue>(key, ChangeType.Add, owner, cancellationToken)
            .ConfigureAwait(false);

        await _permissionService.SetPermissionAsync<TValue>(owner, key, cancellationToken).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<TValue> GetValuesAsync(string owner,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));

        await _values.CreateTableIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

        var permissions = _permissionService.GetPermissionsAsync<TValue>(owner, cancellationToken);
        await foreach (var permission in permissions.ConfigureAwait(false))
        {
            var latest = await _values
                .GetIfExistsAsync(permission.TargetId, CoreConstants.LatestVersion,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (latest == null) continue;

            yield return latest;
        }
    }

    public async Task<TValue?> GetValueAsync(string key, string version, CancellationToken cancellationToken = default)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (version == null) throw new ArgumentNullException(nameof(version));

        await _values.CreateTableIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

        return await _values.GetIfExistsAsync(key, version, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<TValue?> GetLatestValueAsync(string key, CancellationToken cancellationToken = default)
        => await GetValueAsync(key, CoreConstants.LatestVersion, cancellationToken).ConfigureAwait(false);
}