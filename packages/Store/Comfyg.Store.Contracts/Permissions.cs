namespace Comfyg.Store.Contracts;

/// <summary>
/// Permission flags.
/// </summary>
[Flags]
public enum Permissions
{
    // Permission to read.
    Read = 1,

    // Permission to write.
    Write = 2,

    // Permission to delete.
    Delete = 4,

    // Permission to set permissions.
    Permit = 8
}
