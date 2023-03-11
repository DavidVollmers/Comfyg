using System.Text.Json.Serialization;
using Comfyg.Contracts.Authentication;

namespace Comfyg.Contracts.Responses;

public sealed class ConnectionResponse
{
    [JsonConverter(typeof(ContractConverter<IClient, Client>))]
    public IClient Client { get; }

    public ConnectionResponse(IClient client)
    {
        Client = client;
    }
}