namespace Comfyg.Authentication.Abstractions;

public sealed class Client : IClient
{
    public Client(string clientId, string clientSecret, string friendlyName)
    {
        ClientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
        ClientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));
        FriendlyName = friendlyName ?? throw new ArgumentNullException(nameof(friendlyName));
    }

    public string ClientId { get; }

    public string ClientSecret { get; }

    public string FriendlyName { get; }
}