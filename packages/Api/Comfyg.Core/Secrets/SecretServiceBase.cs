using Comfyg.Contracts.Secrets;
using Comfyg.Core.Abstractions.Secrets;
using CoreHelpers.WindowsAzure.Storage.Table;

namespace Comfyg.Core.Secrets;

public abstract class SecretServiceBase : ISecretService
{
    private readonly IStorageContext? _storageContext;

    internal SecretServiceBase(string? systemId, IStorageContext? storageContext)
    {
        _storageContext = storageContext;

        if (_storageContext != null)
        {
        }
    }

    public Task AddSecretValueAsync(string owner, string key, string value)
    {
        if (_storageContext == null) throw new NotImplementedException();
    }

    public Task<IEnumerable<ISecretValue>> GetSecretValuesAsync(string owner)
    {
        if (_storageContext == null) throw new NotImplementedException();
    }

    public Task<ISecretValue?> GetSecretValueAsync(string key, string version = CoreConstants.LatestVersion)
    {
        if (_storageContext == null) throw new NotImplementedException();
    }

    public abstract Task<string> ProtectSecretValueAsync(string value, CancellationToken cancellationToken = default);

    public abstract Task<string> UnprotectSecretValueAsync(string value, CancellationToken cancellationToken = default);
}