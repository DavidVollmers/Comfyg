using Comfyg.Contracts.Authentication;

namespace Comfyg.Authentication.Abstractions;

public interface IClientService
{
    Task<IClient?> GetClientAsync(string clientId);

    Task<string> ReceiveClientSecretAsync(IClient client);

    Task<IClient> CreateClientAsync(IClient client);
}