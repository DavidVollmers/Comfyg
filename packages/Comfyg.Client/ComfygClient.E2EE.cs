using System.Security.Cryptography;
using Comfyg.Store.Contracts;

namespace Comfyg.Client;

public partial class ComfygClient
{
    internal async Task<IEnumerable<T>> EncryptAsync<T>(IEnumerable<T> rawValues, CancellationToken cancellationToken)
        where T : IComfygValue
    {
        if (!_isAsymmetric) throw new InvalidOperationException(E2EeNotSupportedExceptionMessage);

        //TODO
        // 1) GET encryption private key from server
        // 2) decrypt private key with E2EE secret
        // 3) cache locally
        // 4) If no encryption key exists => create new one
    }
}
