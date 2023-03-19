using Comfyg.Contracts.Secrets;
using CoreHelpers.WindowsAzure.Storage.Table.Attributes;

namespace Comfyg.Core.Secrets;

[Storable(nameof(SecretValueEntity))]
internal class SecretValueEntity : ISecretValue, ISerializableComfygValue
{
    [PartitionKey] public string Key { get; set; } = null!;

    public string Value { get; set; } = null!;

    [RowKey] public string Version { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}