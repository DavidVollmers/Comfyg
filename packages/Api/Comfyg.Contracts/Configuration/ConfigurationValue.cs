namespace Comfyg.Contracts.Configuration;

internal class ConfigurationValue : IConfigurationValue
{
    public string Key { get; set; } = null!;
    
    public string Value { get; set; } = null!;
    
    public string Version { get; set; } = null!;
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}