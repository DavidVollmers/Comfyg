namespace Comfyg.Contracts.Settings;

internal class SettingValue : ISettingValue
{
    public string Key { get; set; } = null!;
    
    public string Value { get; set; } = null!;
    
    public string Version { get; set; } = null!;
}