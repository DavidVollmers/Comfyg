using System.Runtime.Serialization;
using Azure.Data.Tables.Poco;
using Comfyg.Core.Abstractions.Permissions;

namespace Comfyg.Core.Permissions;

internal abstract class PermissionEntityBase : IPermission
{
    public string Owner { get; set; } = null!;

    public string TargetId { get; set; } = null!;

    [StoreAsTypeInfo] public Type TargetType { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

internal class PermissionEntity : PermissionEntityBase
{
    [PartitionKey] [IgnoreDataMember] public string Partition => $"{Owner}-{TargetType.FullName}";

    [RowKey] [IgnoreDataMember] public string Row => $"{TargetId}";
}

internal class PermissionEntityMirrored : PermissionEntityBase
{
    [PartitionKey] [IgnoreDataMember] public string Partition => $"{TargetType.FullName}-{TargetId}";

    [RowKey] [IgnoreDataMember] public string Row => $"{Owner}";
}