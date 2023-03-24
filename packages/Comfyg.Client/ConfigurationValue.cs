using Comfyg.Contracts.Configuration;

namespace Comfyg.Client;

public sealed class ConfigurationValue : IConfigurationValue
{
    public ConfigurationValue(string key, string value)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string Key { get; }

    public string Value { get; }

    public string Version => null!;

    public DateTimeOffset CreatedAt => DateTimeOffset.UtcNow;
}
