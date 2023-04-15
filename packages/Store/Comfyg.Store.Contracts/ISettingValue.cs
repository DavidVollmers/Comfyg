using System.Text.Json.Serialization;

namespace Comfyg.Store.Contracts;

/// <summary>
/// A Comfyg setting value. 
/// </summary>
[JsonConverter(typeof(ContractConverter<ISettingValue, Implementation, IComfygValue>))]
public interface ISettingValue : IComfygValue
{
    // ReSharper disable once ClassNeverInstantiated.Local
    private class Implementation : ISettingValue
    {
        public string Key { get; init; } = null!;

        public string Value { get; init; } = null!;

        public string Version { get; init; } = null!;

        public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

        public string Hash { get; init; } = null!;

        public string? Tag { get; init; }
    }
}
