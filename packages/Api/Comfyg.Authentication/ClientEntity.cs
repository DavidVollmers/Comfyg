using Azure.Data.Tables.Poco;
using Comfyg.Contracts.Authentication;

namespace Comfyg.Authentication;

internal class ClientEntity : IClient
{
    [PartitionKey] [RowKey] public string ClientId { get; set; } = null!;

    public string ClientSecret { get; set; } = null!;

    public string FriendlyName { get; set; } = null!;
}