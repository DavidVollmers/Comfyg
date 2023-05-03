using Comfyg.Store.Contracts;

namespace Comfyg.Client;

/// <summary>
/// Represents a Comfyg secret value.
/// </summary>
public sealed class SecretValue : ISecretValue
{
    /// <summary>
    /// Creates a new Comfyg secret value.
    /// </summary>
    /// <param name="key">The key of the secret value.</param>
    /// <param name="value">The secret value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> or <paramref name="value"/> is null.</exception>
    public SecretValue(string key, string value)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// The key of the secret value.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// The secret value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// The version of the secret value. Always returns `null`.
    /// </summary>
    public string Version => null!;

    /// <summary>
    /// The time when the secret value was created.
    /// </summary>
    public DateTimeOffset CreatedAt => DateTimeOffset.UtcNow;

    /// <summary>
    /// A hash value to identity the secret value. Always returns `null`.
    /// </summary>
    public string? Hash => null;

    /// <summary>
    /// The parent version of the secret value. Always returns `null`.
    /// </summary>
    public string? ParentVersion => null;

    public bool IsEncrypted => false;

    internal class Initializer : ISecretValue, IComfygValueInitializer
    {
        public string Key { get; init; } = null!;

        public string Value { get; init; } = null!;

        public string Version { get; init; } = null!;
        
        public DateTimeOffset CreatedAt { get; init; }

        public string? Hash { get; init; }
        
        public string? ParentVersion { get; init; }
        
        public bool IsEncrypted { get; init; }
    }
}
