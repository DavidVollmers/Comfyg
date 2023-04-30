using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Azure.Data.Tables;
using Azure.Data.Tables.Poco;
using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Contracts;
using Comfyg.Store.Core.Abstractions;
using Comfyg.Store.Core.Abstractions.Secrets;

namespace Comfyg.Store.Authentication;

internal class ClientService : IClientService
{
    private const string ClientSecretBlobPrefix = "blob:";

    private readonly TypedTableClient<ClientEntity> _clients;
    private readonly ISecretService _secretService;
    private readonly IBlobService _blobService;

    public ClientService(TableServiceClient tableServiceClient, ISecretService secretService, IBlobService blobService)
    {
        if (tableServiceClient == null) throw new ArgumentNullException(nameof(tableServiceClient));

        _secretService = secretService ?? throw new ArgumentNullException(nameof(secretService));
        _blobService = blobService ?? throw new ArgumentNullException(nameof(blobService));

        _clients = tableServiceClient.GetTableClient<ClientEntity>();
    }

    public async Task<IClient?> GetClientAsync(string clientId, CancellationToken cancellationToken = default)
    {
        await _clients.CreateTableIfNotExistsAsync(cancellationToken);

        return await _clients.GetIfExistsAsync(clientId, clientId, cancellationToken: cancellationToken);
    }

    public async Task<byte[]> ReceiveClientSecretAsync(IClient client, CancellationToken cancellationToken = default)
    {
        if (client.ClientSecret.StartsWith(ClientSecretBlobPrefix))
        {
            var blob = await _blobService.DownloadBlobAsync(client.ClientSecret[ClientSecretBlobPrefix.Length..],
                cancellationToken);

            using var stream = new MemoryStream();
            await blob.CopyToAsync(stream, cancellationToken);

            return stream.ToArray();
        }

        var clientSecret = await _secretService.UnprotectSecretValueAsync(client.ClientSecret, cancellationToken);
        return Convert.FromBase64String(clientSecret);
    }

    public async Task<IClient> CreateSymmetricClientAsync(IClient client, CancellationToken cancellationToken = default)
    {
        var clientSecret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var protectedSecret = await _secretService.ProtectSecretValueAsync(clientSecret, cancellationToken);

        return await InternalCreateClientAsync(client, protectedSecret, cancellationToken);
    }

    public async Task<IClient> CreateAsymmetricClientAsync(IClient client, X509Certificate certificate,
        CancellationToken cancellationToken = default)
    {
        var blobId = $"{client.ClientId}.crt";
        using var blob = new MemoryStream(certificate.Export(X509ContentType.Cert));

        await _blobService.UploadBlobAsync(blobId, blob, cancellationToken: cancellationToken);

        return await InternalCreateClientAsync(client, $"{ClientSecretBlobPrefix}{blobId}", cancellationToken);
    }

    private async Task<IClient> InternalCreateClientAsync(IClient client, string secretReference,
        CancellationToken cancellationToken)
    {
        var clientEntity = new ClientEntity
        {
            ClientId = client.ClientId, FriendlyName = client.FriendlyName, ClientSecret = secretReference
        };

        await _clients.CreateTableIfNotExistsAsync(cancellationToken);

        await _clients.AddAsync(clientEntity, cancellationToken);

        return clientEntity;
    }
}
