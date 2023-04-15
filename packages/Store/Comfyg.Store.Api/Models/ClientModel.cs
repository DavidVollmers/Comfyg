using Comfyg.Store.Contracts;

namespace Comfyg.Store.Api.Models;

internal class ClientModel : IClient
{
    public string ClientId { get; init; }

    public string ClientSecret { get; init; }

    public string FriendlyName { get; init; }

#pragma warning disable CS8618
    public ClientModel() { }
#pragma warning restore CS8618

    public ClientModel(IClient client)
    {
        ClientId = client.ClientId;
        ClientSecret = client.ClientSecret;
        FriendlyName = client.FriendlyName;
    }
}
