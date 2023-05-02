namespace Comfyg.Store.Core.Abstractions;

public interface IBlobService
{
    Task UploadBlobAsync(string blobId, Stream content, bool throwIfExists = true,
        CancellationToken cancellationToken = default);

    Task<Stream> DownloadBlobAsync(string blobId, CancellationToken cancellationToken = default);

    Task<bool> DoesBlobExistAsync(string blobId, CancellationToken cancellationToken = default);
}
