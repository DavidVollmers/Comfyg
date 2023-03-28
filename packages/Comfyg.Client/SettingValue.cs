using Comfyg.Store.Contracts;

namespace Comfyg.Client;

/// <summary>
/// Represents a Comfyg setting value.
/// </summary>
public sealed class SettingValue : ISettingValue
{
    /// <summary>
    /// Creates a new Comfyg setting value.
    /// </summary>
    /// <param name="key">The key of the setting value.</param>
    /// <param name="value">The setting value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> or <paramref name="value"/> is null.</exception>
    public SettingValue(string key, string value)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// The key of the setting value.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// The setting value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// The version of the setting value. Always returns `null`.
    /// </summary>
    public string Version => null!;

    /// <summary>
    /// The time when the setting value was created.
    /// </summary>
    public DateTimeOffset CreatedAt => DateTimeOffset.UtcNow;

    /// <summary>
    /// A hash value to identity the setting value. Always returns `null`.
    /// </summary>
    public string Hash => null!;
}
