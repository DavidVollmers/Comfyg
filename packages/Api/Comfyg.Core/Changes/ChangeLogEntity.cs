using Azure.Data.Tables.Poco;
using Comfyg.Contracts.Changes;

namespace Comfyg.Core.Changes;

internal abstract class ChangeLogEntityBase : IChangeLog
{
    [StoreAsTypeInfo] public Type TargetType { get; init; } = null!;

    private readonly string _targetId = null!;
    
    public string TargetId { get => _targetId; init => _targetId = value.ToLower(); }

    public DateTimeOffset ChangedAt { get; init; } = DateTimeOffset.UtcNow;

    public ChangeType ChangeType { get; init; }

    public string ChangedBy { get; init; } = null!;
}

internal class ChangeLogEntity : ChangeLogEntityBase
{
    [PartitionKey] public string PartitionKey => $"{TargetType.FullName}-{TargetId}";

    [RowKey] public string RowKey => $"{ChangedAt.Ticks}";
}

internal class ChangeLogEntityMirrored : ChangeLogEntityBase
{
    [PartitionKey] public string PartitionKey => $"{TargetType.FullName}";

    [RowKey] public string RowKey => $"{TargetId}-{ChangedAt.Ticks}";
}
