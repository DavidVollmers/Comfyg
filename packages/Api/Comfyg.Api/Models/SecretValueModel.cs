using Comfyg.Contracts;
using Comfyg.Contracts.Secrets;

namespace Comfyg.Api.Models;

public class SecretValueModel : ISecretValue
{
    public string Key { get; set; }

    public string Value { get; set; }

    public string Version { get; set; }

    public DateTime CreatedAt { get; set; }

    public SecretValueModel(IComfygValue value)
    {
        Key = value.Key;
        Value = value.Value;
        Version = value.Version;
        CreatedAt = value.CreatedAt;
    }
}