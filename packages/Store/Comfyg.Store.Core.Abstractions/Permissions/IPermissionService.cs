namespace Comfyg.Store.Core.Abstractions.Permissions;

public interface IPermissionService
{
    Task<bool> IsPermittedAsync<T>(string owner, string targetId, Contracts.Permissions permissions,
        bool allowCreate = false, CancellationToken cancellationToken = default);

    IAsyncEnumerable<IPermission> GetPermissionsAsync<T>(string owner,
        Contracts.Permissions? requiredPermissions = null, CancellationToken cancellationToken = default);

    Task SetPermissionAsync<T>(string owner, string targetId, Contracts.Permissions permissions,
        CancellationToken cancellationToken = default);
}
