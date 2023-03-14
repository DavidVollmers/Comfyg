using Comfyg.Contracts.Configuration;

namespace Comfyg.Client;

public sealed class ConfigurationValue : IConfigurationValue
{
    public ConfigurationValue(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public string Key { get; }

    public string Value { get; }

    public string Version => null!;
    
    public DateTime CreatedAt => DateTime.UtcNow;
}