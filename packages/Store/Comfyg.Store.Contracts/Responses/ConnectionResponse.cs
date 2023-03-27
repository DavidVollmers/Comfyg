using System.Text.Json.Serialization;
using Comfyg.Store.Contracts.Authentication;

namespace Comfyg.Store.Contracts.Responses;

/// <summary>
/// Response object returned when establishing a connection to a Comfyg store.
/// </summary>
public sealed class ConnectionResponse
{
    /// <summary>
    /// The Comfyg client used to establish the connection.
    /// </summary>
    [JsonConverter(typeof(ContractConverter<IClient, Client>))]
    public IClient Client { get; }

    /// <summary>
    /// Creates a new instance of the response object.
    /// </summary>
    /// <param name="client">The client used to establish the connection.</param>
    /// <exception cref="ArgumentNullException"><paramref name="client"/> is null.</exception>
    public ConnectionResponse(IClient client)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
    }
}
