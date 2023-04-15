using System.Text.Json.Serialization;

namespace Comfyg.Store.Contracts;

/// <summary>
/// A Comfyg secret value.
/// </summary>
[JsonConverter(typeof(ContractConverter<ISecretValue, Implementation, IComfygValue>))]
public interface ISecretValue : IComfygValue
{
    // ReSharper disable once ClassNeverInstantiated.Local
    private class Implementation : ISecretValue
    {
        public string Key { get; init; } = null!;

        public string Value { get; init; } = null!;

        public string Version { get; init; } = null!;

        public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

        public string Hash { get; init; } = null!;

        public string? ParentVersion { get; init; }
    }
}
