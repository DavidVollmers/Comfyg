using Comfyg.Contracts.Settings;
using CoreHelpers.WindowsAzure.Storage.Table.Attributes;

namespace Comfyg.Core.Settings;

[Storable(nameof(SettingValueEntity))]
internal class SettingValueEntity : ISettingValue, ISerializableComfygValue
{
    [PartitionKey] public string Key { get; set; } = null!;

    public string Value { get; set; } = null!;

    [RowKey] public string Version { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}