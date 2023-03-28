using System.Text.Json.Serialization;

namespace Comfyg.Store.Contracts.Responses;

/// <summary>
/// Response object returned when establishing a connection to a Comfyg store.
/// </summary>
[JsonConverter(typeof(ContractConverter<IConnectionResponse, Implementation>))]
public interface IConnectionResponse
{
    /// <summary>
    /// The Comfyg client used to establish the connection.
    /// </summary>
    IClient Client { get; }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class Implementation : IConnectionResponse
    {
        public IClient Client { get; init; } = null!;
    }
}
