namespace Comfyg.Store.Core.Abstractions.Permissions;

[Flags]
public enum Permissions
{
    Read = 1,
    Write = 2,
    Delete = 4,
    Permit = 8
}
