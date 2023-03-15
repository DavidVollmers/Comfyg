using Comfyg.Contracts.Authentication;
using CoreHelpers.WindowsAzure.Storage.Table.Attributes;

namespace Comfyg.Authentication;

[Storable(nameof(ClientEntity))]
internal class ClientEntity : IClient
{
    [PartitionKey] [RowKey] public string ClientId { get; set; } = null!;

    public string ClientSecret { get; set; } = null!;

    public string FriendlyName { get; set; } = null!;
}