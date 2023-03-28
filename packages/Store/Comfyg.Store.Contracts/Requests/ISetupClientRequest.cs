using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Comfyg.Store.Contracts.Requests;

/// <summary>
/// Request object used to setup a new Comfyg client.
/// </summary>
[JsonConverter(typeof(ContractConverter<ISetupClientRequest, Implementation>))]
public interface ISetupClientRequest
{
    /// <summary>
    /// The Comfyg client to register. 
    /// </summary>
    [Required]
    IClient Client { get; }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class Implementation : ISetupClientRequest
    {
        public IClient Client { get; init; } = null!;
    }
}
