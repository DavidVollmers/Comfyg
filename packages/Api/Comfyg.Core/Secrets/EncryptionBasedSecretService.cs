using Comfyg.Core.Abstractions.Secrets;

namespace Comfyg.Core.Secrets;

public sealed class EncryptionBasedSecretService : ISecretService
{
    private readonly string _encryptionKey;

    public EncryptionBasedSecretService(string encryptionKey)
    {
        _encryptionKey = encryptionKey ?? throw new ArgumentNullException(nameof(encryptionKey));
    }

    public Task<string> ProtectSecretValueAsync(string value)
    {
        throw new NotImplementedException();
    }

    public Task<string> UnprotectSecretValueAsync(string value)
    {
        throw new NotImplementedException();
    }
}