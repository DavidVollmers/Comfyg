using System.Text.Json.Serialization;
using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Authentication;
using Comfyg.Store.Contracts.Responses;

namespace Comfyg.Client.Responses;

internal class SetupClientResponse : ISetupClientResponse
{
    [JsonConverter(typeof(ContractConverter<IClient, Client>))]
    public IClient Client { get; init; } = null!;

    public string ClientSecret { get; init; } = null!;
}
