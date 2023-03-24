using System.Security.Cryptography;
using Comfyg.Core.Abstractions.Secrets;

namespace Comfyg.Core.Secrets;

public sealed class EncryptionBasedSecretService : ISecretService
{
    private const string IvDelimiter = ".";

    private readonly byte[] _encryptionKey;

    public EncryptionBasedSecretService(string encryptionKey)
    {
        if (encryptionKey == null) throw new ArgumentNullException(nameof(encryptionKey));

        _encryptionKey = Convert.FromBase64String(encryptionKey);
    }

    public async Task<string> ProtectSecretValueAsync(string value, CancellationToken cancellationToken = default)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        using var aes = Aes.Create();

        var encryptor = aes.CreateEncryptor(_encryptionKey, aes.IV);

        using var stream = new MemoryStream();
        var crypto = new CryptoStream(stream, encryptor, CryptoStreamMode.Write);
        var writer = new StreamWriter(crypto);
        await writer.WriteAsync(value);
        await writer.DisposeAsync();
        await crypto.DisposeAsync();

        return Convert.ToBase64String(aes.IV) + IvDelimiter + Convert.ToBase64String(stream.ToArray());
    }

    public async Task<string> UnprotectSecretValueAsync(string value, CancellationToken cancellationToken = default)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        byte[] iv;
        byte[] data;
        try
        {
            var parts = value.Split(IvDelimiter);
            iv = Convert.FromBase64String(parts[0]);
            data = Convert.FromBase64String(parts[1]);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException("Value is not correctly encrypted.", exception);
        }

        using var aes = Aes.Create();

        var decryptor = aes.CreateDecryptor(_encryptionKey, iv);

        using var stream = new MemoryStream(data);
        await using var crypto = new CryptoStream(stream, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(crypto);
        return await reader.ReadToEndAsync();
    }
}
