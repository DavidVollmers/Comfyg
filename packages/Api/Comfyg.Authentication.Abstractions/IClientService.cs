using Comfyg.Contracts.Authentication;

namespace Comfyg.Authentication.Abstractions;

public interface IClientService
{
    Task<IClient?> GetClientAsync(string clientId, CancellationToken cancellationToken = default);

    Task<string> ReceiveClientSecretAsync(IClient client, CancellationToken cancellationToken = default);

    Task<IClient> CreateClientAsync(IClient client, CancellationToken cancellationToken = default);
}