namespace Comfyg.Contracts.Configuration;

public sealed class ConfigurationValue : IConfigurationValue
{
    public string Key { get; set; } = null!;
    
    public string Value { get; set; } = null!;
    
    public string Version { get; set; } = null!;
    
    public string[] Tags { get; set; } = null!;
}