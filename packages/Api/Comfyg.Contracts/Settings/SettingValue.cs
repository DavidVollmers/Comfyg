namespace Comfyg.Contracts.Settings;

public class SettingValue : ISettingValue
{
    public string Key { get; set; } = null!;
    
    public string Value { get; set; } = null!;
    
    public string Version { get; set; } = null!;
}