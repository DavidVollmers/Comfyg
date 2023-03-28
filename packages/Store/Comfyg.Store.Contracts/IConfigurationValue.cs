using System.Text.Json.Serialization;

namespace Comfyg.Store.Contracts;

/// <summary>
/// A Comfyg configuration value.
/// </summary>
[JsonConverter(typeof(ContractConverter<IConfigurationValue, Implementation, IComfygValue>))]
public interface IConfigurationValue : IComfygValue
{
    // ReSharper disable once ClassNeverInstantiated.Local
    private class Implementation : IConfigurationValue
    {
        public string Key { get; init; } = null!;

        public string Value { get; init; } = null!;

        public string Version { get; init; } = null!;

        public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

        public string Hash { get; init; } = null!;
    }
}
