using Comfyg.Store.Contracts.Authentication;

namespace Comfyg.Store.Contracts.Responses;

/// <summary>
/// Response object returned when establishing a connection to a Comfyg store.
/// </summary>
public interface IConnectionResponse
{
    /// <summary>
    /// The Comfyg client used to establish the connection.
    /// </summary>
    IClient Client { get; }
}
