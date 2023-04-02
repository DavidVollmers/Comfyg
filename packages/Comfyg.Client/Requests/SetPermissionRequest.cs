using Comfyg.Store.Contracts.Requests;

namespace Comfyg.Client.Requests;

internal class SetPermissionRequest : ISetPermissionRequest
{
    public string ClientId { get; }
    
    public string Key { get; }

    public SetPermissionRequest(string clientId, string key)
    {
        ClientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
        Key = key ?? throw new ArgumentNullException(nameof(key));
    }
}
