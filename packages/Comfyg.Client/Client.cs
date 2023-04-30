using Comfyg.Store.Contracts;

namespace Comfyg.Client;

/// <summary>
/// Represents a Comfyg client.
/// </summary>
public sealed class Client : IClient
{
    /// <summary>
    /// Creates a new client object. 
    /// </summary>
    /// <param name="clientId">The client ID.</param>
    /// <param name="friendlyName">The user friendly display name of the client.</param>
    /// <exception cref="ArgumentNullException"><paramref name="clientId"/> or <paramref name="friendlyName"/> is null.</exception>
    public Client(string clientId, string friendlyName, bool isAsymmetric)
    {
        ClientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
        FriendlyName = friendlyName ?? throw new ArgumentNullException(nameof(friendlyName));
        IsAsymmetric = isAsymmetric;
    }

    /// <summary>
    /// The client ID.
    /// </summary>
    public string ClientId { get; }

    /// <summary>
    /// The client secret. Always returns `null`.
    /// </summary>
    public string ClientSecret => null!;

    /// <summary>
    /// The user friendly display name of the client.
    /// </summary>
    public string FriendlyName { get; }

    /// <summary>
    /// Specifies if the client uses an asymmetric client secret.
    /// </summary>
    public bool IsAsymmetric { get; }
}
