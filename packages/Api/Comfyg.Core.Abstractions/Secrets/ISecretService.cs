using Comfyg.Contracts.Secrets;

namespace Comfyg.Core.Abstractions.Secrets;

public interface ISecretService
{
    Task<string> ProtectSecretValueAsync(string value, CancellationToken cancellationToken = default);

    Task<string> UnprotectSecretValueAsync(string value, CancellationToken cancellationToken = default);
    
    Task AddSecretValueAsync(string owner, string key, string value);

    Task<IEnumerable<ISecretValue>> GetSecretValuesAsync(string owner);

    Task<ISecretValue?> GetSecretValueAsync(string key, string version = null!);
}