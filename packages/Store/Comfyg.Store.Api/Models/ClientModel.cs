using Comfyg.Store.Contracts.Authentication;

namespace Comfyg.Store.Api.Models;

internal class ClientModel : IClient
{
    public string ClientId { get; init; }

    public string ClientSecret { get; init; }

    public string FriendlyName { get; init; }

    public ClientModel() { }

    public ClientModel(IClient client)
    {
        ClientId = client.ClientId;
        ClientSecret = client.ClientSecret;
        FriendlyName = client.FriendlyName;
    }
}
