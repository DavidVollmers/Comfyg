namespace Comfyg.Core.Abstractions.Permissions;

public sealed class Permission<T> : IPermission
{
    public string Owner { get; set; } = null!;

    public string TargetId { get; set; } = null!;

    public string TargetType => typeof(T).FullName!;
}