using Azure.Data.Tables.Poco;
using Comfyg.Contracts.Authentication;

namespace Comfyg.Authentication;

internal class ClientEntity : IClient
{
    [PartitionKey][RowKey] public string ClientId { get; init; } = null!;

    public string ClientSecret { get; init; } = null!;

    public string FriendlyName { get; init; } = null!;
}
