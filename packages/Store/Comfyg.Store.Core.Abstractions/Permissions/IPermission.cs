namespace Comfyg.Store.Core.Abstractions.Permissions;

public interface IPermission
{
    string Owner { get; }

    string TargetId { get; }

    Type TargetType { get; }

    DateTimeOffset CreatedAt { get; }
}
