namespace Comfyg.Core.Abstractions.Permissions;

public interface IPermissionService
{
    Task<bool> IsPermittedAsync<T>(string owner, string targetId);

    Task<IEnumerable<IPermission>> GetPermissionsAsync<T>(string owner);

    Task SetPermissionAsync<T>(string owner, string targetId);
}