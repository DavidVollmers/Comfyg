using System.Text.Json.Serialization;
using Comfyg.Store.Contracts.Authentication;

namespace Comfyg.Store.Contracts.Responses;

/// <summary>
/// Response object returned when setting up a new Comfyg client.
/// </summary>
public sealed class SetupClientResponse
{
    /// <summary>
    /// The Comfyg client which was registered.
    /// </summary>
    [JsonConverter(typeof(ContractConverter<IClient, Client>))]
    public IClient Client { get; }

    /// <summary>
    /// The generated client secret of the registered Comfyg client.
    /// </summary>
    public string ClientSecret { get; }

    /// <summary>
    /// Creates a new response object instance.
    /// </summary>
    /// <param name="client">The client which was registered.</param>
    /// <param name="clientSecret">The client secret of the registered Comfyg client.</param>
    /// <exception cref="ArgumentNullException"><paramref name="client"/> or <paramref name="clientSecret"/> is null.</exception>
    public SetupClientResponse(IClient client, string clientSecret)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
        ClientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));
    }
}
