using CoreHelpers.WindowsAzure.Storage.Table.Attributes;

namespace Comfyg.Core.Configuration;

//TODO refactor into ownerPermission (for generic permissions on config, settings, secrets)
internal abstract class ConfigurationValueOwnerEntityBase
{
    public string Owner { get; set; } = null!;
    
    public string Key { get; set; } = null!;
}

[Storable(nameof(ConfigurationValueOwnerEntity))]
[VirtualPartitionKey(nameof(Owner))]
[VirtualRowKey(nameof(Key))]
internal class ConfigurationValueOwnerEntity : ConfigurationValueOwnerEntityBase
{
}

[Storable(nameof(ConfigurationValueOwnerEntityMirrored))]
[VirtualPartitionKey(nameof(Key))]
[VirtualRowKey(nameof(Owner))]
internal class ConfigurationValueOwnerEntityMirrored : ConfigurationValueOwnerEntityBase
{
}