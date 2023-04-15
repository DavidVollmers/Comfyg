using Azure.Data.Tables.Poco;

namespace Comfyg.Store.Core;

internal abstract class ComfygValueEntity : IComfygValueInitializer
{
    private readonly string _key = null!;

    [PartitionKey] public string Key { get => _key; init => _key = value.ToLower(); }

    public string Value { get; init; } = null!;

    public string Version { get; init; } = null!;

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public string Hash { get; init; } = null!;

    public string? Tag { get; init; } = null;

    [RowKey]
    public string RowKey
    {
        get
        {
            var version = Version;
            if (Tag != null) version += "-" + Tag;
            return version;
        }
    }
}
