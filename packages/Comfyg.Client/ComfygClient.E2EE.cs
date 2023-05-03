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

        var encryptionKey = await GetEncryptionKeyAsync(cancellationToken);
        if (encryptionKey == null)
        {
            throw new InvalidOperationException("End to end-encryption is not properly setup.");
        }

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

        using var aes = Aes.Create();

        var decryptedValue = await DecryptValueAsync(aes, encryptedValue.Value, encryptionKey);
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

    internal async Task<IEnumerable<TEncrypted>> EncryptAsync<TRaw, TEncrypted>(IEnumerable<TRaw> rawValues,
        CancellationToken cancellationToken)
        where TRaw : IComfygValue
        where TEncrypted : class, TRaw, IComfygValueInitializer, new()
    {
        if (rawValues == null) throw new ArgumentNullException(nameof(rawValues));

        if (!_isAsymmetric) throw new InvalidOperationException(E2EeNotSupportedExceptionMessage);

        var encryptionKey = await GetEncryptionKeyAsync(cancellationToken) ??
                            await CreateEncryptionKeyAsync(cancellationToken);

        using var aes = Aes.Create();
        using var encryptor = aes.CreateEncryptor(encryptionKey, aes.IV);

        var results = new List<TEncrypted>();
        foreach (var rawValue in rawValues)
        {
            if (!rawValue.IsEncrypted)
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

            var encryptedValue = await EncryptValueAsync(encryptor, rawValue.Value);
            results.Add(new TEncrypted
            {
                Value = encryptedValue,
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

    private async Task<byte[]> CreateEncryptionKeyAsync(CancellationToken cancellationToken)
    {
        using var aes = Aes.Create();
        using var encryptor = aes.CreateEncryptor(_e2EeSecret!, aes.IV);

        using var stream = new MemoryStream();
        var crypto = new CryptoStream(stream, encryptor, CryptoStreamMode.Write);
        await crypto.WriteAsync(aes.Key, cancellationToken);
        await crypto.DisposeAsync();

        stream.SetLength(0);
        var content = $"{Convert.ToBase64String(stream.ToArray())}{IvDelimiter}{Convert.ToBase64String(aes.IV)}";
        var writer = new StreamWriter(stream);
        await writer.WriteAsync(content);
        await writer.DisposeAsync();

        var response =
            await SendRequestAsync(
                () => new HttpRequestMessage(HttpMethod.Post, "encryption/key") {Content = new StreamContent(stream)},
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

        var encryptedKey = Convert.FromBase64String(parts[0]);
        using var stream = new MemoryStream(encryptedKey);
        var crypto = new CryptoStream(stream, decryptor, CryptoStreamMode.Read);
        var length = await crypto.ReadAsync(encryptedKey, cancellationToken);

        return _encryptionKey = stream.ToArray().Take(length).ToArray();
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

    private static async Task<string> DecryptValueAsync(SymmetricAlgorithm alg, string encryptedValue,
        byte[] encryptionKey)
    {
        var parts = encryptedValue.Split(IvDelimiter);

        using var decryptor = alg.CreateDecryptor(encryptionKey, Convert.FromBase64String(parts[1]));

        using var stream = new MemoryStream(Convert.FromBase64String(parts[0]));
        await using var crypto = new CryptoStream(stream, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(crypto);
        return await reader.ReadToEndAsync();
    }
}
