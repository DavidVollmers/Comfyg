namespace Comfyg.Store.Contracts.Responses;

/// <summary>
/// Response object returned when setting up a new Comfyg client.
/// </summary>
public interface ISetupClientResponse
{
    /// <summary>
    /// The Comfyg client which was registered.
    /// </summary>
    IClient Client { get; }

    /// <summary>
    /// The generated client secret of the registered Comfyg client.
    /// </summary>
    string ClientSecret { get; }
}
