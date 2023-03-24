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
    [PartitionKey] public string PartitionKey => $"{Owner}-{TargetType.FullName}";

    [RowKey] public string RowKey => $"{TargetId}";
}

internal class PermissionEntityMirrored : PermissionEntityBase
{
    [PartitionKey] public string PartitionKey => $"{TargetType.FullName}-{TargetId}";

    [RowKey] public string RowKey => $"{Owner}";
}
