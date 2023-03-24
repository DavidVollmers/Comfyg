namespace Comfyg.Contracts.Secrets;

internal class SecretValue : ISecretValue
{
    public string Key { get; init; } = null!;

    public string Value { get; init; } = null!;

    public string Version { get; init; } = null!;

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public string Hash { get; init; } = null!;
}
