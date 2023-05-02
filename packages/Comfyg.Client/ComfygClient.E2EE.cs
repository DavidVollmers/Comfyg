using System.Net;
using System.Security.Cryptography;
using Comfyg.Store.Contracts;

namespace Comfyg.Client;

public partial class ComfygClient
{
    private byte[]? _e2EeKey;
    private byte[]? _e2EeIv;

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
        using var encryptor = aes.CreateEncryptor(e2EeKey, e2EeIv!);

        var results = new List<TEncrypted>();
        foreach (var rawValue in rawValues)
        {
            var encryptedValue = await EncryptValueAsync(encryptor, rawValue.Value);
            results.Add(new TEncrypted {Value = encryptedValue, Key = rawValue.Key});
        }

        return results;
    }

    private async Task<(byte[], byte[])> CreateEncryptionKeyAsync(CancellationToken cancellationToken)
    {
        using var aes = Aes.Create();

        _e2EeKey = aes.Key;
        _e2EeIv = aes.IV;

        using var encryptor = aes.CreateEncryptor(_e2EeSecret!, _e2EeSecretIv!);

        using var stream = new MemoryStream();
        var crypto = new CryptoStream(stream, encryptor, CryptoStreamMode.Write);
        var writer = new StreamWriter(crypto);
        await writer.WriteAsync($"{Convert.ToBase64String(_e2EeKey)}{IvDelimiter}{Convert.ToBase64String(_e2EeIv)}");
        await writer.DisposeAsync();
        await crypto.DisposeAsync();

        var response =
            await SendRequestAsync(
                () => new HttpRequestMessage(HttpMethod.Post, "encryption/key") {Content = new StreamContent(stream)},
                cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("Invalid status code when trying to set encryption key.", null,
                response.StatusCode);
        }

        return (_e2EeKey, _e2EeIv);
    }

    private async Task<(byte[]?, byte[]?)> GetEncryptionKeyAsync(CancellationToken cancellationToken)
    {
        if (_e2EeKey != null && _e2EeIv != null) return (_e2EeKey, _e2EeIv);

        var response = await SendRequestAsync(() => new HttpRequestMessage(HttpMethod.Get, "encryption/key"),
            cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.NotFound) return (null, null);

            throw new HttpRequestException("Invalid status code when trying to get encryption key.", null,
                response.StatusCode);
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        using var aes = Aes.Create();
        using var decryptor = aes.CreateDecryptor(_e2EeSecret!, _e2EeSecretIv!);

        await using var crypto = new CryptoStream(stream, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(crypto);
        // ReSharper disable once MethodSupportsCancellation
#pragma warning disable CA2016
        var e2EeKey = await reader.ReadToEndAsync();
#pragma warning restore CA2016

        var parts = e2EeKey.Split(IvDelimiter);
        _e2EeKey = Convert.FromBase64String(parts[0]);
        _e2EeIv = Convert.FromBase64String(parts[1]);

        return (_e2EeKey, _e2EeIv);
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
