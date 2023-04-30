using Azure.Storage.Blobs;
using Comfyg.Store.Core.Abstractions;

namespace Comfyg.Store.Core;

public sealed class BlobService : IBlobService
{
    private readonly string _container;
    private readonly BlobServiceClient _blobServiceClient;

    public BlobService(string container, BlobServiceClient blobServiceClient)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container));
        _blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
    }

    public async Task UploadBlobAsync(string blobId, Stream content, bool throwIfExists = true,
        CancellationToken cancellationToken = default)
    {
        if (blobId == null) throw new ArgumentNullException(nameof(blobId));
        if (content == null) throw new ArgumentNullException(nameof(content));

        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_container);

        if (!await blobContainerClient.ExistsAsync(cancellationToken))
            await blobContainerClient.CreateAsync(cancellationToken: cancellationToken);

        var blobClient = blobContainerClient.GetBlobClient(blobId);

        if (throwIfExists && await blobClient.ExistsAsync(cancellationToken))
            throw new InvalidOperationException($"Blob already exists: {blobId}");

        await blobClient.UploadAsync(content, !throwIfExists, cancellationToken);
    }

    public async Task<Stream> DownloadBlobAsync(string blobId, CancellationToken cancellationToken = default)
    {
        if (blobId == null) throw new ArgumentNullException(nameof(blobId));
        
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_container);

        if (!await blobContainerClient.ExistsAsync(cancellationToken))
            await blobContainerClient.CreateAsync(cancellationToken: cancellationToken);

        var blobClient = blobContainerClient.GetBlobClient(blobId);

        if (!await blobClient.ExistsAsync(cancellationToken)) throw new InvalidOperationException($"Blob does not exist: {blobId}");

        var blob = await blobClient.DownloadAsync(cancellationToken);
        return blob.Value.Content;
    }
}
