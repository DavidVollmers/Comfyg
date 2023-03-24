using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Comfyg.Contracts.Authentication;

namespace Comfyg.Contracts.Requests;

public sealed class SetupClientRequest
{
    [Required]
    [JsonConverter(typeof(ContractConverter<IClient, Client>))]
    public IClient Client { get; init; } = null!;
}
