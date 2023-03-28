using Comfyg.Store.Contracts;

namespace Comfyg.Store.Api.Models;

internal class SecretValueModel : ISecretValue
{
    public string Key { get; init; }

    public string Value { get; init; }

    public string Version { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public string Hash { get; init; }

    public SecretValueModel() { }

    public SecretValueModel(IComfygValue value)
    {
        Key = value.Key;
        Value = value.Value;
        Version = value.Version;
        CreatedAt = value.CreatedAt;
        Hash = value.Hash;
    }
}
