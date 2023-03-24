namespace Comfyg.Core.Abstractions.Secrets;

public interface ISecretService
{
    Task<string> ProtectSecretValueAsync(string value, CancellationToken cancellationToken = default);

    Task<string> UnprotectSecretValueAsync(string value, CancellationToken cancellationToken = default);
}
