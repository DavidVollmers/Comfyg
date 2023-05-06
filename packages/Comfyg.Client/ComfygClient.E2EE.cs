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
        if (encryptedValue == null) throw new ArgumentNullException(nameof(encryptedValue));

        if (!_isAsymmetric) throw new InvalidOperationException(E2EeNotSupportedExceptionMessage);

        if (!encryptedValue.IsEncrypted)
            return new TDecrypted
            {
                Value = encryptedValue.Value,
                Key = encryptedValue.Key,
                Version = encryptedValue.Version,
                ParentVersion = encryptedValue.ParentVersion,
                CreatedAt = encryptedValue.CreatedAt,
                Hash = encryptedValue.Hash,
                IsEncrypted = false
            };

        var encryptionKey = await GetEncryptionKeyAsync(cancellationToken).ConfigureAwait(false);

        if (encryptionKey == null) throw new InvalidOperationException("Missing encryption key.");

        using var aes = Aes.Create();

        try
        {
            var decryptedValue =
                await DecryptValueAsync(aes, encryptedValue.Value, encryptionKey).ConfigureAwait(false);
            return new TDecrypted
            {
                Value = decryptedValue,
                Key = encryptedValue.Key,
                Version = encryptedValue.Version,
                ParentVersion = encryptedValue.ParentVersion,
                CreatedAt = encryptedValue.CreatedAt,
                Hash = encryptedValue.Hash,
                IsEncrypted = false
            };
        }
        catch (CryptographicException exception)
        {
            throw new InvalidOperationException(
                "Cannot decrypt value. This could be the case because a different encryption key was used to encrypt it.",
                exception);
        }
    }

    internal async Task<IEnumerable<TEncrypted>> EncryptAsync<TRaw, TEncrypted>(IEnumerable<TRaw> rawValues,
        CancellationToken cancellationToken)
        where TRaw : IComfygValue
        where TEncrypted : class, TRaw, IComfygValueInitializer, new()
    {
        if (rawValues == null) throw new ArgumentNullException(nameof(rawValues));

        if (!_isAsymmetric) throw new InvalidOperationException(E2EeNotSupportedExceptionMessage);

        var encryptionKey = await GetEncryptionKeyAsync(cancellationToken).ConfigureAwait(false);

        if (encryptionKey == null) throw new InvalidOperationException("Missing encryption key.");

        using var aes = Aes.Create();
        using var encryptor = aes.CreateEncryptor(encryptionKey, aes.IV);

        var base64Iv = Convert.ToBase64String(aes.IV);

        var results = new List<TEncrypted>();
        foreach (var rawValue in rawValues)
        {
            if (rawValue.IsEncrypted)
            {
                results.Add(new TEncrypted
                {
                    Value = rawValue.Value,
                    Key = rawValue.Key,
                    Version = rawValue.Version,
                    ParentVersion = rawValue.ParentVersion,
                    CreatedAt = rawValue.CreatedAt,
                    Hash = rawValue.Hash,
                    IsEncrypted = true
                });
                continue;
            }

            var encryptedValue = await EncryptValueAsync(encryptor, rawValue.Value).ConfigureAwait(false);
            results.Add(new TEncrypted
            {
                Value = $"{encryptedValue}{IvDelimiter}{base64Iv}",
                Key = rawValue.Key,
                Version = rawValue.Version,
                ParentVersion = rawValue.ParentVersion,
                CreatedAt = rawValue.CreatedAt,
                Hash = rawValue.Hash,
                IsEncrypted = true
            });
        }

        return results;
    }

    private async Task<byte[]?> GetEncryptionKeyAsync(CancellationToken cancellationToken)
    {
        if (_encryptionKey != null) return UseEncryptionKey();

        var response = await SendRequestAsync(() => new HttpRequestMessage(HttpMethod.Get, "encryption/key"),
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.NotFound) return null;

            if (!_isAsymmetric && response.StatusCode == HttpStatusCode.Forbidden)
                throw new InvalidOperationException(E2EeNotSupportedExceptionMessage);

            throw new HttpRequestException("Invalid status code when trying to get encryption key.", null,
                response.StatusCode);
        }

        await using var responseStream =
            await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        using var stream = new MemoryStream();
        await responseStream.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
        _encryptionKey = stream.ToArray();

        return UseEncryptionKey();
    }

    private byte[] UseEncryptionKey()
    {
        using var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(_clientSecret, out _);

        return rsa.Decrypt(_encryptionKey!, RSAEncryptionPadding.Pkcs1);
    }

    private static async Task<string> EncryptValueAsync(ICryptoTransform encryptor, string rawValue)
    {
        using var stream = new MemoryStream();
        var crypto = new CryptoStream(stream, encryptor, CryptoStreamMode.Write);
        var writer = new StreamWriter(crypto);
        await writer.WriteAsync(rawValue).ConfigureAwait(false);
        await writer.DisposeAsync().ConfigureAwait(false);
        await crypto.DisposeAsync().ConfigureAwait(false);

        return Convert.ToBase64String(stream.ToArray());
    }

    private static async Task<string> DecryptValueAsync(SymmetricAlgorithm alg, string encryptedValue,
        byte[] encryptionKey)
    {
        var parts = encryptedValue.Split(IvDelimiter);

        using var decryptor = alg.CreateDecryptor(encryptionKey, Convert.FromBase64String(parts[1]));

        using var stream = new MemoryStream(Convert.FromBase64String(parts[0]));
        await using var crypto = new CryptoStream(stream, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(crypto);
        return await reader.ReadToEndAsync().ConfigureAwait(false);
    }
}
