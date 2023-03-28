using System.Text.Json.Serialization;
using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Authentication;
using Comfyg.Store.Contracts.Responses;

namespace Comfyg.Client.Responses;

internal class ConnectionResponse : IConnectionResponse
{
    [JsonConverter(typeof(ContractConverter<IClient, Client>))]
    public IClient Client { get; init; } = null!;
}
