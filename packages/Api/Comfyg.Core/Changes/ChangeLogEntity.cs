using System.Runtime.Serialization;
using Comfyg.Contracts.Changes;
using CoreHelpers.WindowsAzure.Storage.Table.Attributes;

namespace Comfyg.Core.Changes;

internal abstract class ChangeLogEntityBase : IChangeLog
{
    public string TargetType { get; set; } = null!;

    public string TargetId { get; set; } = null!;

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    public ChangeType ChangeType { get; set; } = ChangeType.Unknown;

    public long ChangedAtKey => long.MaxValue - ChangedAt.Ticks;

    public string ChangedBy { get; set; } = null!;
}

[Storable(nameof(ChangeLogEntity))]
[VirtualPartitionKey($"{{{{{nameof(TargetType)}}}}}-{{{{{nameof(TargetId)}}}}}")]
[VirtualRowKey(nameof(ChangedAtKey))]
internal class ChangeLogEntity : ChangeLogEntityBase
{
}

[Storable(nameof(ChangeLogEntityMirrored))]
[VirtualPartitionKey(nameof(TargetType))]
[VirtualRowKey($"{{{{{nameof(TargetId)}}}}}-{{{{{nameof(ChangedAtKey)}}}}}")]
internal class ChangeLogEntityMirrored : ChangeLogEntityBase
{
}