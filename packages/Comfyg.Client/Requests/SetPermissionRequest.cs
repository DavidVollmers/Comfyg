using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Requests;

namespace Comfyg.Client.Requests;

internal class SetPermissionRequest : ISetPermissionRequest
{
    public string ClientId { get; }

    public string Key { get; }

    public Permissions Permissions { get; set; }

    public SetPermissionRequest(string clientId, string key, Permissions permissions)
    {
        ClientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Permissions = permissions;
    }
}
