namespace Comfyg.Store.Core.Abstractions.Permissions;

public interface IPermissionService
{
    Task<bool> IsPermittedAsync<T>(string owner, string targetId, Permissions permissions, bool mustExist = true,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<IPermission> GetPermissionsAsync<T>(string owner, Permissions? requiredPermissions = null,
        CancellationToken cancellationToken = default);

    Task SetPermissionAsync<T>(string owner, string targetId, Permissions permissions,
        CancellationToken cancellationToken = default);
}
