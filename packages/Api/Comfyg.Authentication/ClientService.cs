using Comfyg.Authentication.Abstractions;

namespace Comfyg.Authentication;

internal class ClientService : IClientService
{
    public Task<IClient> GetClientAsync(string clientId)
    {
        throw new NotImplementedException();
    }

    public Task<string> ReceiveClientSecretAsync(IClient client)
    {
        throw new NotImplementedException();
    }
}