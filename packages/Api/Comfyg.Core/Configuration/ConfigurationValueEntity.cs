using Azure.Data.Tables.Poco;
using Comfyg.Contracts.Configuration;

namespace Comfyg.Core.Configuration;

internal class ConfigurationValueEntity : IConfigurationValue, ISerializableComfygValue
{
    [PartitionKey] public string Key { get; set; } = null!;

    public string Value { get; set; } = null!;

    [RowKey] public string Version { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;
}