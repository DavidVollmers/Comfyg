namespace Comfyg.Store.Authentication;

public sealed class ComfygAuthenticationOptions
{
    private const string UseEitherEncryptionOrKeyVaultExceptionMessage =
        "Cannot use encryption and Azure Key Vault. Use either ComfygAuthenticationOptions.UseEncryption or ComfygAuthenticationOptions.UseKeyVault to configure secret handling.";

    internal string? AzureTableStorageConnectionString { get; private set; }
    
    internal string? AzureBlobStorageConnectionString { get; private set; }

    internal string? EncryptionKey { get; private set; }

    internal Uri? KeyVaultUri { get; private set; }

    internal ComfygAuthenticationOptions()
    {
    }

    public ComfygAuthenticationOptions UseAzureTableStorage(string connectionString)
    {
        AzureTableStorageConnectionString =
            connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        return this;
    }

    public ComfygAuthenticationOptions UseAzureBlobStorage(string connectionString)
    {
        AzureBlobStorageConnectionString =
            connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        return this;
    }

    public ComfygAuthenticationOptions UseEncryption(string encryptionKey)
    {
        if (KeyVaultUri != null) throw new InvalidOperationException(UseEitherEncryptionOrKeyVaultExceptionMessage);
        EncryptionKey = encryptionKey ?? throw new ArgumentNullException(nameof(encryptionKey));
        return this;
    }

    public ComfygAuthenticationOptions UseAzureKeyVault(string vaultUri)
    {
        if (EncryptionKey != null) throw new InvalidOperationException(UseEitherEncryptionOrKeyVaultExceptionMessage);
        if (vaultUri == null) throw new ArgumentNullException(nameof(vaultUri));
        KeyVaultUri = new Uri(vaultUri);
        return this;
    }
}
