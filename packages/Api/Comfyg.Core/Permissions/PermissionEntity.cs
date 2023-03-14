using Comfyg.Core.Abstractions.Permissions;
using CoreHelpers.WindowsAzure.Storage.Table.Attributes;

namespace Comfyg.Core.Permissions;

internal abstract class PermissionEntityBase : IPermission
{
    public string Owner { get; set; } = null!;

    public string TargetId { get; set; } = null!;

    public string TargetType { get; set; } = null!;
}

[Storable(nameof(PermissionEntity))]
[VirtualPartitionKey($"{{{{{nameof(Owner)}}}}}-{{{{{nameof(TargetType)}}}}}")]
[VirtualRowKey(nameof(TargetId))]
internal class PermissionEntity : PermissionEntityBase
{
}

[Storable(nameof(PermissionEntityMirrored))]
[VirtualPartitionKey($"{{{{{nameof(TargetType)}}}}}-{{{{{nameof(TargetId)}}}}}")]
[VirtualRowKey(nameof(Owner))]
internal class PermissionEntityMirrored : PermissionEntityBase
{
}