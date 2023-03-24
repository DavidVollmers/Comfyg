using Azure.Data.Tables.Poco;

namespace Comfyg.Core;

internal abstract class ComfygValueEntity : IComfygValueInitializer
{
    private readonly string _key = null!;

    [PartitionKey] public string Key { get => _key; init => _key = value.ToLower(); }

    public string Value { get; init; } = null!;

    [RowKey] public string Version { get; init; } = null!;

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
