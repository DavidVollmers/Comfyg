using System.Text.Json.Serialization;

namespace Comfyg.Store.Contracts.Responses;

/// <summary>
/// Response object returned when setting up a new Comfyg client.
/// </summary>
[JsonConverter(typeof(ContractConverter<ISetupClientResponse, Implementation>))]
public interface ISetupClientResponse
{
    /// <summary>
    /// The Comfyg client which was registered.
    /// </summary>
    IClient Client { get; }

    /// <summary>
    /// The generated client secret of the registered Comfyg client. If an asymmetric client secret is used this will always return `null`.
    /// </summary>
    string? ClientSecret { get; }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class Implementation : ISetupClientResponse
    {
        public IClient Client { get; init; } = null!;

        public string? ClientSecret { get; init; }
    }
}
