﻿using Comfyg.Contracts.Changes;
using Comfyg.Core.Abstractions.Changes;
using Comfyg.Core.Abstractions.Permissions;
using CoreHelpers.WindowsAzure.Storage.Table;

namespace Comfyg.Core.Changes;

internal class ChangeService : IChangeService
{
    private readonly IStorageContext _storageContext;
    private readonly IPermissionService _permissionService;

    public ChangeService(string systemId, IStorageContext storageContext, IPermissionService permissionService)
    {
        _storageContext = storageContext;
        _permissionService = permissionService;

        _storageContext.AddAttributeMapper<ChangeLogEntity>();
        _storageContext.OverrideTableName<ChangeLogEntity>($"{systemId}{nameof(ChangeLogEntity)}");
        _storageContext.AddAttributeMapper<ChangeLogEntityMirrored>();
        _storageContext.OverrideTableName<ChangeLogEntityMirrored>($"{systemId}{nameof(ChangeLogEntityMirrored)}");
    }

    public async Task LogChangeAsync<T>(string targetId, ChangeType changeType, string changedBy)
    {
        using var context = _storageContext.CreateChildContext();
        context.EnableAutoCreateTable();

        await context.InsertOrReplaceAsync(new ChangeLogEntity
        {
            TargetId = targetId,
            ChangeType = changeType,
            TargetType = typeof(T).FullName!,
            ChangedBy = changedBy
        }).ConfigureAwait(false);
        await context.InsertOrReplaceAsync(new ChangeLogEntityMirrored
        {
            TargetId = targetId,
            ChangeType = changeType,
            TargetType = typeof(T).FullName!,
            ChangedBy = changedBy
        }).ConfigureAwait(false);
    }

    public async Task<IEnumerable<IChangeLog>> GetChangesSinceAsync<T>(DateTime since)
    {
        using var context = _storageContext.CreateChildContext();

        return await context.EnableAutoCreateTable().QueryAsync<ChangeLogEntityMirrored>(typeof(T).FullName, new[]
        {
            new QueryFilter
            {
                Property = nameof(ChangeLogEntityBase.ChangedAtKey),
                Operator = QueryFilterOperator.LowerEqual,
                Value = long.MaxValue - since.Ticks
            }
        }).ConfigureAwait(false);
    }

    public async Task<IEnumerable<IChangeLog>> GetChangesForOwnerAsync<T>(string owner, DateTime since)
    {
        var permissions = await _permissionService.GetPermissionsAsync<T>(owner).ConfigureAwait(false);

        var changes = await GetChangesSinceAsync<T>(since).ConfigureAwait(false);

        return changes.Where(c => permissions.Any(p => p.TargetId == c.TargetId));
    }
}