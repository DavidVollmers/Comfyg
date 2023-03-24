using System.Runtime.CompilerServices;
using Azure.Data.Tables;
using Azure.Data.Tables.Poco;
using Comfyg.Contracts.Changes;
using Comfyg.Core.Abstractions.Changes;
using Comfyg.Core.Abstractions.Permissions;

namespace Comfyg.Core.Changes;

internal class ChangeService : IChangeService
{
    private readonly TypedTableClient<ChangeLogEntity> _changeLog;
    private readonly TypedTableClient<ChangeLogEntityMirrored> _changeLogMirrored;
    private readonly IPermissionService _permissionService;

    public ChangeService(string systemId, TableServiceClient tableServiceClient, IPermissionService permissionService)
    {
        if (systemId == null) throw new ArgumentNullException(nameof(systemId));
        if (tableServiceClient == null) throw new ArgumentNullException(nameof(tableServiceClient));

        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));

        _changeLog = tableServiceClient.GetTableClient<ChangeLogEntity>()
            .OverrideTableName($"{systemId}{nameof(ChangeLogEntity)}");
        _changeLogMirrored = tableServiceClient.GetTableClient<ChangeLogEntityMirrored>()
            .OverrideTableName($"{systemId}{nameof(ChangeLogEntityMirrored)}");
    }

    public async Task LogChangeAsync<T>(string targetId, ChangeType changeType, string changedBy,
        CancellationToken cancellationToken = default)
    {
        if (targetId == null) throw new ArgumentNullException(nameof(targetId));
        if (changedBy == null) throw new ArgumentNullException(nameof(changedBy));

        await _changeLog.CreateTableIfNotExistsAsync(cancellationToken).ConfigureAwait(false);
        await _changeLog
            .AddAsync(
                new ChangeLogEntity
                {
                    TargetId = targetId,
                    ChangeType = changeType,
                    TargetType = typeof(T),
                    ChangedBy = changedBy
                }, cancellationToken).ConfigureAwait(false);

        await _changeLogMirrored.CreateTableIfNotExistsAsync(cancellationToken).ConfigureAwait(false);
        await _changeLogMirrored
            .AddAsync(
                new ChangeLogEntityMirrored
                {
                    TargetId = targetId,
                    ChangeType = changeType,
                    TargetType = typeof(T),
                    ChangedBy = changedBy
                }, cancellationToken).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<IChangeLog> GetChangesSinceAsync<T>(DateTimeOffset since,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await _changeLogMirrored.CreateTableIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

        var changedAtFilter =
            TypedTableClient<ChangeLogEntityMirrored>.CreateQueryFilter(c => c.ChangedAt >= since.ToUniversalTime());
        var filter = $"PartitionKey eq '{typeof(T).FullName}' and {changedAtFilter}";

        var changes = _changeLogMirrored.QueryAsync(filter, cancellationToken: cancellationToken);
        await foreach (var change in changes.WithCancellation(cancellationToken).ConfigureAwait(false))
            yield return change;
    }

    public async IAsyncEnumerable<IChangeLog> GetChangesForOwnerAsync<T>(string owner, DateTimeOffset since,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));

        var permissions = await _permissionService.GetPermissionsAsync<T>(owner, cancellationToken)
            .ToArrayAsync(cancellationToken).ConfigureAwait(false);

        var changes = GetChangesSinceAsync<T>(since, cancellationToken)
            .Where(c => permissions.Any(p => p.TargetId == c.TargetId));
        await foreach (var change in changes.WithCancellation(cancellationToken).ConfigureAwait(false))
            yield return change;
    }
}
