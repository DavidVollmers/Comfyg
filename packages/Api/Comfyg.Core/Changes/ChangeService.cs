using Comfyg.Contracts.Changes;
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
        if (systemId == null) throw new ArgumentNullException(nameof(systemId));

        _storageContext = storageContext ?? throw new ArgumentNullException(nameof(storageContext));
        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));

        _storageContext.AddAttributeMapper<ChangeLogEntity>($"{systemId}{nameof(ChangeLogEntity)}");
        _storageContext.AddAttributeMapper<ChangeLogEntityMirrored>($"{systemId}{nameof(ChangeLogEntityMirrored)}");
    }

    public async Task LogChangeAsync<T>(string targetId, ChangeType changeType, string changedBy)
    {
        if (targetId == null) throw new ArgumentNullException(nameof(targetId));
        if (changedBy == null) throw new ArgumentNullException(nameof(changedBy));

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
        if (owner == null) throw new ArgumentNullException(nameof(owner));

        var permissions = await _permissionService.GetPermissionsAsync<T>(owner).ConfigureAwait(false);

        var changes = await GetChangesSinceAsync<T>(since).ConfigureAwait(false);

        return changes.Where(c => permissions.Any(p => p.TargetId == c.TargetId));
    }
}