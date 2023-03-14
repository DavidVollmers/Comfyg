namespace Comfyg.Core.Abstractions.Permissions;

public interface IPermission
{
    string Owner { get; }
    
    string TargetId { get; }
    
    string TargetType { get; }
}