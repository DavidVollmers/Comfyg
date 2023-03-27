using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Comfyg.Store.Contracts.Authentication;

namespace Comfyg.Store.Contracts.Requests;

/// <summary>
/// Request object used to setup a new Comfyg client.
/// </summary>
public sealed class SetupClientRequest
{
    /// <summary>
    /// The Comfyg client to register. 
    /// </summary>
    [Required]
    [JsonConverter(typeof(ContractConverter<IClient, Client>))]
    public IClient Client { get; init; } = null!;
}
