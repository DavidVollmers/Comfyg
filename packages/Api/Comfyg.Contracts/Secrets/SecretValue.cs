namespace Comfyg.Contracts.Secrets;

public class SecretValue : ISecretValue
{
    public string Key { get; set; } = null!;

    public string Value { get; set; } = null!;

    public string Version { get; set; } = null!;
}