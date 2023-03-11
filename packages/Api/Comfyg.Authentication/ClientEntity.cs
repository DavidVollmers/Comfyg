using Comfyg.Contracts.Authentication;
using CoreHelpers.WindowsAzure.Storage.Table.Attributes;

namespace Comfyg.Authentication;

[Storable(nameof(ClientEntity))]
internal class ClientEntity : IClient
{
    [PartitionKey] [RowKey] public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string FriendlyName { get; set; }

    public ClientEntity()
    {
    }

    public ClientEntity(IClient client)
    {
        ClientId = client.ClientId;
        ClientSecret = client.ClientSecret;
        FriendlyName = client.FriendlyName;
    }
}