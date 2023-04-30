using Azure.Data.Tables.Poco;
using Comfyg.Store.Contracts;

namespace Comfyg.Store.Authentication;

internal class ClientEntity : IClient
{
    [PartitionKey][RowKey] public string ClientId { get; init; } = null!;

    public string ClientSecret { get; init; } = null!;

    public string FriendlyName { get; init; } = null!;
    
    public bool IsAsymmetric { get; init; }
}
