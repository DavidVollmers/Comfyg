using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Configuration;

namespace Comfyg.Store.Api.Models;

internal class ConfigurationValueModel : IConfigurationValue
{
    public string Key { get; init; }

    public string Value { get; init; }

    public string Version { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public string Hash { get; init; }

    public ConfigurationValueModel() { }

    public ConfigurationValueModel(IComfygValue value)
    {
        Key = value.Key;
        Value = value.Value;
        Version = value.Version;
        CreatedAt = value.CreatedAt;
        Hash = value.Hash;
    }
}
