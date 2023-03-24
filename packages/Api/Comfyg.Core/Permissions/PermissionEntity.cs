using Azure.Data.Tables.Poco;
using Comfyg.Core.Abstractions.Permissions;

namespace Comfyg.Core.Permissions;

internal abstract class PermissionEntityBase : IPermission
{
    public string Owner { get; init; } = null!;

    private readonly string _targetId = null!;

    public string TargetId { get => _targetId; init => _targetId = value.ToLower(); }

    [StoreAsTypeInfo] public Type TargetType { get; init; } = null!;

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
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
