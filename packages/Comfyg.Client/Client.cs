using Comfyg.Store.Contracts.Authentication;

namespace Comfyg.Client;

/// <summary>
/// Represents a Comfyg client.
/// </summary>
public sealed class Client : IClient
{
    /// <summary>
    /// Creates a new client object. 
    /// </summary>
    /// <param name="clientId">The ID of the client.</param>
    /// <param name="friendlyName">The user friendly display name of the client.</param>
    /// <exception cref="ArgumentNullException"><paramref name="clientId"/> or <paramref name="friendlyName"/> is null.</exception>
    public Client(string clientId, string friendlyName)
    {
        ClientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
        FriendlyName = friendlyName ?? throw new ArgumentNullException(nameof(friendlyName));
    }

    /// <summary>
    /// The client ID of the client.
    /// </summary>
    public string ClientId { get; }

    /// <summary>
    /// The client secret of the client. Always returns `null`.
    /// </summary>
    public string ClientSecret => null!;

    /// <summary>
    /// The user friendly display name of the client.
    /// </summary>
    public string FriendlyName { get; }
}
