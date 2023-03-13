using System.Runtime.Serialization;
using Comfyg.Contracts.Configuration;
using CoreHelpers.WindowsAzure.Storage.Table.Attributes;

namespace Comfyg.Core.Configuration;

[Storable(nameof(ConfigurationValueEntity))]
internal class ConfigurationValueEntity : IConfigurationValue
{
    public ConfigurationValueEntity() {}
    
    public ConfigurationValueEntity(IConfigurationValue configurationValue)
    {
        Key = configurationValue.Key;
        Value = configurationValue.Value;
        Version = configurationValue.Version;
    }

    [PartitionKey] public string Key { get; set; } = null!;

    public string Value { get; set; } = null!;

    [RowKey] public string Version { get; set; } = null!;

    [IgnoreDataMember] public string[] Tags { get; } = Array.Empty<string>();
}