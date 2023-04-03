using Comfyg.Store.Contracts.Requests;

namespace Comfyg.Client.Requests;

internal class SetPermissionsRequest : ISetPermissionsRequest
{
    public string ClientId { get; }

    public SetPermissionsRequest(string clientId)
    {
        ClientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
    }
}
