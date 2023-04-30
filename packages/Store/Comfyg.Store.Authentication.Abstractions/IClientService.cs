using System.Security.Cryptography.X509Certificates;
using Comfyg.Store.Contracts;

namespace Comfyg.Store.Authentication.Abstractions;

public interface IClientService
{
    Task<IClient?> GetClientAsync(string clientId, CancellationToken cancellationToken = default);

    Task<byte[]> ReceiveClientSecretAsync(IClient client, CancellationToken cancellationToken = default);

    Task<IClient> CreateSymmetricClientAsync(IClient client, CancellationToken cancellationToken = default);

    Task<IClient> CreateAsymmetricClientAsync(IClient client, X509Certificate certificate,
        CancellationToken cancellationToken = default);
}
