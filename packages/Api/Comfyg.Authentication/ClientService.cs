using System.Security.Cryptography;
using Comfyg.Authentication.Abstractions;
using Comfyg.Contracts.Authentication;
using Comfyg.Core.Abstractions.Secrets;
using CoreHelpers.WindowsAzure.Storage.Table;

namespace Comfyg.Authentication;

internal class ClientService : IClientService
{
    private readonly IStorageContext _storageContext;
    private readonly ISecretService _secretService;

    public ClientService(IStorageContext storageContext, ISecretService secretService)
    {
        _storageContext = storageContext;
        _secretService = secretService;

        _storageContext.AddAttributeMapper<ClientEntity>();
    }

    public async Task<IClient?> GetClientAsync(string clientId)
    {
        using var context = _storageContext.CreateChildContext();
        return await context.EnableAutoCreateTable().QueryAsync<ClientEntity>(clientId, clientId, 1)
            .ConfigureAwait(false);
    }

    public async Task<string> ReceiveClientSecretAsync(IClient client)
    {
        return await _secretService.UnprotectSecretValueAsync(client.ClientSecret).ConfigureAwait(false);
    }

    public async Task<IClient> CreateClientAsync(IClient client)
    {
        var clientSecret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var protectedSecret = await _secretService.ProtectSecretValueAsync(clientSecret).ConfigureAwait(false);

        var clientEntity = new ClientEntity
        {
            ClientId = client.ClientId,
            FriendlyName = client.FriendlyName,
            ClientSecret = protectedSecret
        };

        using var context = _storageContext.CreateChildContext();
        await context.EnableAutoCreateTable().InsertOrReplaceAsync(clientEntity).ConfigureAwait(false);

        return clientEntity;
    }
}