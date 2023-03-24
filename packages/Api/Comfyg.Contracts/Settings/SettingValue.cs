namespace Comfyg.Contracts.Settings;

internal class SettingValue : ISettingValue
{
    public string Key { get; init; } = null!;

    public string Value { get; init; } = null!;

    public string Version { get; init; } = null!;

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
