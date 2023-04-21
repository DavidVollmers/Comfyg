using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Azure.Data.Tables;
using Azure.Data.Tables.Poco;
using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Contracts;
using Comfyg.Store.Core.Abstractions.Secrets;

namespace Comfyg.Store.Authentication;

internal class ClientService : IClientService
{
    private readonly TypedTableClient<ClientEntity> _clients;
    private readonly ISecretService _secretService;

    public ClientService(TableServiceClient tableServiceClient, ISecretService secretService)
    {
        if (tableServiceClient == null) throw new ArgumentNullException(nameof(tableServiceClient));

        _secretService = secretService ?? throw new ArgumentNullException(nameof(secretService));

        _clients = tableServiceClient.GetTableClient<ClientEntity>();
    }

    public async Task<IClient?> GetClientAsync(string clientId, CancellationToken cancellationToken = default)
    {
        await _clients.CreateTableIfNotExistsAsync(cancellationToken);

        return await _clients.GetIfExistsAsync(clientId, clientId, cancellationToken: cancellationToken);
    }

    public async Task<string> ReceiveClientSecretAsync(IClient client, CancellationToken cancellationToken = default)
    {
        return await _secretService.UnprotectSecretValueAsync(client.ClientSecret, cancellationToken);
    }

    public async Task<IClient> CreateSymmetricClientAsync(IClient client, CancellationToken cancellationToken = default)
    {
        var clientSecret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var protectedSecret = await _secretService.ProtectSecretValueAsync(clientSecret, cancellationToken);

        var clientEntity = new ClientEntity
        {
            ClientId = client.ClientId,
            FriendlyName = client.FriendlyName,
            ClientSecret = protectedSecret
        };

        await _clients.CreateTableIfNotExistsAsync(cancellationToken);

        await _clients.AddAsync(clientEntity, cancellationToken);

        return clientEntity;
    }

    public Task<IClient> CreateAsymmetricClientAsync(IClient client, X509Certificate certificate,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
