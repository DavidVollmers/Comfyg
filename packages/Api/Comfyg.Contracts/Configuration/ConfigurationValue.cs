namespace Comfyg.Contracts.Configuration;

internal class ConfigurationValue : IConfigurationValue
{
    public string Key { get; set; } = null!;
    
    public string Value { get; set; } = null!;
    
    public string Version { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}