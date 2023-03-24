using Comfyg.Contracts.Authentication;

namespace Comfyg.Client;

public sealed class Client : IClient
{
    public Client(string clientId, string friendlyName)
    {
        ClientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
        FriendlyName = friendlyName ?? throw new ArgumentNullException(nameof(friendlyName));
    }

    public string ClientId { get; }

    public string ClientSecret => null!;

    public string FriendlyName { get; }
}
