using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Comfyg.Store.Api.Models;
using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Authentication;

namespace Comfyg.Store.Api.Requests;

public sealed class SetupClientRequest
{
    [Required]
    [JsonConverter(typeof(ContractConverter<IClient, ClientModel>))]
    public IClient Client { get; init; } = null!;
}
