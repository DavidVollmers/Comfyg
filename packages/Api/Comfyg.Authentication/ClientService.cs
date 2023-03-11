using Comfyg.Authentication.Abstractions;
using Comfyg.Contracts.Authentication;
using CoreHelpers.WindowsAzure.Storage.Table;

namespace Comfyg.Authentication;

internal class ClientService : IClientService
{
    private readonly IStorageContext _storageContext;

    public ClientService(IStorageContext storageContext)
    {
        _storageContext = storageContext;

        _storageContext.AddAttributeMapper<ClientEntity>();
    }

    public async Task<IClient> GetClientAsync(string clientId)
    {
        using var context = _storageContext.CreateChildContext();
        return await context.QueryAsync<ClientEntity>(clientId, clientId, 1).ConfigureAwait(false);
    }

    public Task<string> ReceiveClientSecretAsync(IClient client)
    {
        throw new NotImplementedException();
    }
}