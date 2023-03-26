using System.Text.Json.Serialization;
using Comfyg.Store.Contracts.Authentication;

namespace Comfyg.Store.Contracts.Responses;

public sealed class ConnectionResponse
{
    [JsonConverter(typeof(ContractConverter<IClient, Client>))]
    public IClient Client { get; }

    public ConnectionResponse(IClient client)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
    }
}
