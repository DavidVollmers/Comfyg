using System.Net;
using System.Security.Cryptography;
using Comfyg.Store.Contracts;

namespace Comfyg.Client;

public partial class ComfygClient
{
    private byte[]? _e2EeKey;
    
    internal async Task<IEnumerable<T>> EncryptAsync<T>(IEnumerable<T> rawValues, CancellationToken cancellationToken)
        where T : IComfygValue
    {
        if (!_isAsymmetric) throw new InvalidOperationException(E2EeNotSupportedExceptionMessage);

        var e2EeKey = await GetEncryptionKeyAsync(cancellationToken);
        if (e2EeKey == null)
        {
            //TODO create E2EE key
        }

        //TODO encrypt values using E2EE key
    }

    private async Task<byte[]?> GetEncryptionKeyAsync(CancellationToken cancellationToken)
    {
        if (_e2EeKey != null) return _e2EeKey;
        
        var response = await SendRequestAsync(() => new HttpRequestMessage(HttpMethod.Get, "encryption/key"),
            cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            
            throw new HttpRequestException("Invalid status code when trying to get encryption key.", null,
                response.StatusCode);
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        using var aes = Aes.Create();
        //TODO set Initialization Vector
        using var decryptor = aes.CreateDecryptor(_e2EeSecret!, _e2EeSecretIv!);
        
        await using var crypto = new CryptoStream(stream, decryptor, CryptoStreamMode.Read);
        using var memoryStream = new MemoryStream();
        await crypto.CopyToAsync(memoryStream, cancellationToken);

        memoryStream.Position = 0;

        return _e2EeKey = memoryStream.ToArray();
    }
}
