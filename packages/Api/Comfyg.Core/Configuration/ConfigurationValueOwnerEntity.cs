using CoreHelpers.WindowsAzure.Storage.Table.Attributes;

namespace Comfyg.Core.Configuration;

[Storable(nameof(ConfigurationValueOwnerEntity))]
internal class ConfigurationValueOwnerEntity
{
    [PartitionKey] public string Owner { get; set; } = null!;
    
    [RowKey] public string Key { get; set; } = null!;
}