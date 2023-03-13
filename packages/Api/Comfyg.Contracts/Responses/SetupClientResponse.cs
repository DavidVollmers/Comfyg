using System.Text.Json.Serialization;
using Comfyg.Contracts.Authentication;

namespace Comfyg.Contracts.Responses;

public sealed class SetupClientResponse
{
    [JsonConverter(typeof(ContractConverter<IClient, Client>))]
    public IClient Client { get; }

    public string ClientSecret { get; set; }

    public SetupClientResponse(IClient client, string clientSecret)
    {
        Client = client;
        ClientSecret = clientSecret;
    }
}