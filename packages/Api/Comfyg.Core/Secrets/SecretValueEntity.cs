using Azure.Data.Tables.Poco;
using Comfyg.Contracts.Secrets;

namespace Comfyg.Core.Secrets;

internal class SecretValueEntity : ISecretValue, ISerializableComfygValue
{
    [PartitionKey] public string Key { get; set; } = null!;

    public string Value { get; set; } = null!;

    [RowKey] public string Version { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}