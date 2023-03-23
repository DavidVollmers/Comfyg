using System.Security.Cryptography;
using Azure.Data.Tables;
using Azure.Data.Tables.Poco;
using Comfyg.Authentication.Abstractions;
using Comfyg.Contracts.Authentication;
using Comfyg.Core.Abstractions.Secrets;

namespace Comfyg.Authentication;

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
        await _clients.CreateTableIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

        return await _clients.GetIfExistsAsync(clientId, clientId, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<string> ReceiveClientSecretAsync(IClient client, CancellationToken cancellationToken = default)
    {
        return await _secretService.UnprotectSecretValueAsync(client.ClientSecret, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IClient> CreateClientAsync(IClient client, CancellationToken cancellationToken = default)
    {
        var clientSecret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var protectedSecret = await _secretService.ProtectSecretValueAsync(clientSecret, cancellationToken)
            .ConfigureAwait(false);

        var clientEntity = new ClientEntity
        {
            ClientId = client.ClientId,
            FriendlyName = client.FriendlyName,
            ClientSecret = protectedSecret
        };

        await _clients.CreateTableIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

        await _clients.AddAsync(clientEntity, cancellationToken).ConfigureAwait(false);

        return clientEntity;
    }
}