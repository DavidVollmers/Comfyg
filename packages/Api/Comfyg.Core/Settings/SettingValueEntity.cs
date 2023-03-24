using Azure.Data.Tables.Poco;
using Comfyg.Contracts.Settings;

namespace Comfyg.Core.Settings;

internal class SettingValueEntity : ISettingValue, ISerializableComfygValue
{
    [PartitionKey] public string Key { get; set; } = null!;

    public string Value { get; set; } = null!;

    [RowKey] public string Version { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
