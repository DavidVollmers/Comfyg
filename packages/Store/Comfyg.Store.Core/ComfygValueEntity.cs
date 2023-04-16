using Azure.Data.Tables.Poco;

namespace Comfyg.Store.Core;

internal abstract class ComfygValueEntity : IComfygValueInitializer
{
    private readonly string _key = null!;

    [PartitionKey] public string Key { get => _key; init => _key = value.ToLower(); }

    public string Value { get; init; } = null!;

    private readonly string _version = null!;

    [RowKey] public string Version { get => _version; init => _version = value.ToLower(); }

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public string Hash { get; init; } = null!;

    public string? ParentVersion { get; init; }
}
