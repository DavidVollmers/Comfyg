using System.Runtime.Serialization;
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
    [PartitionKey] [IgnoreDataMember] public string Partition => $"{TargetType.FullName}-{TargetId}";

    [RowKey] [IgnoreDataMember] public string Row => $"{ChangedAt}";
}

internal class ChangeLogEntityMirrored : ChangeLogEntityBase
{
    [PartitionKey] [IgnoreDataMember] public string Partition => $"{TargetType.FullName}";

    [RowKey] [IgnoreDataMember] public string Row => $"{TargetId}-{ChangedAt}";
}