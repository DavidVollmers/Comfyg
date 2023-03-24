using Comfyg.Contracts.Secrets;

namespace Comfyg.Client;

public sealed class SecretValue : ISecretValue
{
    public SecretValue(string key, string value)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string Key { get; }

    public string Value { get; }

    public string Version => null!;

    public DateTimeOffset CreatedAt => DateTimeOffset.UtcNow;

    public string Hash => null!;
}
