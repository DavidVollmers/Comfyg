using Comfyg.Store.Contracts;

namespace Comfyg.Client;

/// <summary>
/// Represents a Comfyg configuration value.
/// </summary>
public sealed class ConfigurationValue : IConfigurationValue
{
    /// <summary>
    /// Creates a new Comfyg configuration value.
    /// </summary>
    /// <param name="key">The key of the configuration value.</param>
    /// <param name="value">The configuration value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> or <paramref name="value"/> is null.</exception>
    public ConfigurationValue(string key, string value)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// The key of the configuration value.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// The configuration value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// The version of the configuration value. Always returns `null`.
    /// </summary>
    public string Version => null!;

    /// <summary>
    /// The time when the configuration value was created.
    /// </summary>
    public DateTimeOffset CreatedAt => DateTimeOffset.UtcNow;

    /// <summary>
    /// A hash value to identity the configuration value. Always returns `null`.
    /// </summary>
    public string Hash => null!;

    /// <summary>
    /// The tag of the configuration value. Always returns `null`.
    /// </summary>
    public string? Tag => null;
}
