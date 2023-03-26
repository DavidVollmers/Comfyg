namespace Comfyg.Store.Core.Abstractions.Permissions;

public interface IPermissionService
{
    Task<bool> IsPermittedAsync<T>(string owner, string targetId, CancellationToken cancellationToken = default);

    IAsyncEnumerable<IPermission> GetPermissionsAsync<T>(string owner, CancellationToken cancellationToken = default);

    Task SetPermissionAsync<T>(string owner, string targetId, CancellationToken cancellationToken = default);
}
