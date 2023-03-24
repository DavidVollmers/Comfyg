using Azure.Data.Tables.Poco;
using Comfyg.Contracts.Changes;

namespace Comfyg.Core.Changes;

internal abstract class ChangeLogEntityBase : IChangeLog
{
    [StoreAsTypeInfo] public Type TargetType { get; set; } = null!;

    public string TargetId { get; set; } = null!;

    public DateTimeOffset ChangedAt { get; set; } = DateTimeOffset.UtcNow;

    public ChangeType ChangeType { get; set; }

    public string ChangedBy { get; set; } = null!;
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