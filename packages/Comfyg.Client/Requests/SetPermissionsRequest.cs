using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Requests;

namespace Comfyg.Client.Requests;

internal class SetPermissionsRequest : ISetPermissionsRequest
{
    public string ClientId { get; }

    public Permissions Permissions { get; set; }

    public SetPermissionsRequest(string clientId, Permissions permissions)
    {
        ClientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
        Permissions = permissions;
    }
}
