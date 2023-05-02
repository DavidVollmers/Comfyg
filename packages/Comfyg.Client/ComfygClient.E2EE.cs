using System.Net;
using System.Security.Cryptography;
using Comfyg.Store.Contracts;

namespace Comfyg.Client;

public partial class ComfygClient
{
    private byte[]? _encryptionKey;

    internal async Task<TDecrypted> DecryptAsync<TEncrypted, TDecrypted>(TEncrypted encryptedValue,
        CancellationToken cancellationToken)
        where TEncrypted : IComfygValue
        where TDecrypted : class, TEncrypted, IComfygValueInitializer, new()
    {
        if (!_isAsymmetric) throw new InvalidOperationException(E2EeNotSupportedExceptionMessage);

        var (e2EeKey, e2EeIv) = await GetEncryptionKeyAsync(cancellationToken);
        if (e2EeKey == null)
        {
            throw new InvalidOperationException("End to end-encryption is not properly setup.");
        }

        using var aes = Aes.Create();
        using var decryptor = aes.CreateDecryptor(e2EeKey, e2EeIv);

        //TODO decrypt value
    }

    internal async Task<IEnumerable<TEncrypted>> EncryptAsync<TRaw, TEncrypted>(IEnumerable<TRaw> rawValues,
        CancellationToken cancellationToken)
        where TRaw : IComfygValue
        where TEncrypted : class, TRaw, IComfygValueInitializer, new()
    {
        if (!_isAsymmetric) throw new InvalidOperationException(E2EeNotSupportedExceptionMessage);

        var (e2EeKey, e2EeIv) = await GetEncryptionKeyAsync(cancellationToken);
        if (e2EeKey == null)
        {
            (e2EeKey, e2EeIv) = await CreateEncryptionKeyAsync(cancellationToken);
        }

        using var aes = Aes.Create();
        using var encryptor = aes.CreateEncryptor(e2EeKey, e2EeIv);

        var results = new List<TEncrypted>();
        foreach (var rawValue in rawValues)
        {
            var encryptedValue = await EncryptValueAsync(encryptor, rawValue.Value);
            results.Add(new TEncrypted {Value = encryptedValue, Key = rawValue.Key});
        }

        return results;
    }

    private async Task<byte[]> CreateEncryptionKeyAsync(CancellationToken cancellationToken)
    {
        using var aes = Aes.Create();
        using var encryptor = aes.CreateEncryptor(_e2EeSecret!, aes.IV);

        using var stream = new MemoryStream();
        var crypto = new CryptoStream(stream, encryptor, CryptoStreamMode.Write);
        await crypto.WriteAsync(aes.Key, cancellationToken);
        await crypto.DisposeAsync();

        var payload = $"{Convert.ToBase64String(stream.ToArray())}{IvDelimiter}{Convert.ToBase64String(aes.IV)}";
        using var payloadStream = new MemoryStream();
        var writer = new StreamWriter(payloadStream);
        await writer.WriteAsync(payload);
        await writer.DisposeAsync();

        var response =
            await SendRequestAsync(
                () => new HttpRequestMessage(HttpMethod.Post, "encryption/key")
                {
                    Content = new StreamContent(payloadStream)
                },
                cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("Invalid status code when trying to set encryption key.", null,
                response.StatusCode);
        }

        return _encryptionKey = aes.Key;
    }

    private async Task<byte[]?> GetEncryptionKeyAsync(CancellationToken cancellationToken)
    {
        if (_encryptionKey != null) return _encryptionKey;

        var response = await SendRequestAsync(() => new HttpRequestMessage(HttpMethod.Get, "encryption/key"),
            cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.NotFound) return null;

            throw new HttpRequestException("Invalid status code when trying to get encryption key.", null,
                response.StatusCode);
        }

        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        var parts = payload.Split(IvDelimiter);
        
        using var aes = Aes.Create();
        using var decryptor = aes.CreateDecryptor(_e2EeSecret!, Convert.FromBase64String(parts[1]));

        using var stream = new MemoryStream(Convert.FromBase64String(parts[0]));
        await using var crypto = new CryptoStream(stream, decryptor, CryptoStreamMode.Read);

        //TODO crypto => byte[]
        return _encryptionKey;
    }

    private static async Task<string> EncryptValueAsync(ICryptoTransform encryptor, string rawValue)
    {
        using var stream = new MemoryStream();
        var crypto = new CryptoStream(stream, encryptor, CryptoStreamMode.Write);
        var writer = new StreamWriter(crypto);
        await writer.WriteAsync(rawValue);
        await writer.DisposeAsync();
        await crypto.DisposeAsync();

        return Convert.ToBase64String(stream.ToArray());
    }
}
