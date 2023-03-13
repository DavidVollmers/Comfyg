namespace Comfyg.Core.Abstractions.Secrets;

public interface ISecretService
{
    Task<string> ProtectSecretValueAsync(string value);

    Task<string> UnprotectSecretValueAsync(string value);
}