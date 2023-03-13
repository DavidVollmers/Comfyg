namespace Comfyg.Authentication;

public sealed class ComfygAuthenticationOptions
{
    internal string? AzureTableStorageConnectionString { get; private set; }

    internal string? EncryptionKey { get; private set; }

    internal ComfygAuthenticationOptions()
    {
    }

    public ComfygAuthenticationOptions UseAzureTableStorage(string connectionString)
    {
        AzureTableStorageConnectionString =
            connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        return this;
    }

    public ComfygAuthenticationOptions UseEncryption(string encryptionKey)
    {
        EncryptionKey = encryptionKey ?? throw new ArgumentNullException(nameof(encryptionKey));
        return this;
    }
}