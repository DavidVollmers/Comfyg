namespace Comfyg.Store.Core;

public sealed class ComfygOptions
{
    private const string UseEitherEncryptionOrKeyVaultExceptionMessage =
        "Cannot use encryption and Azure Key Vault. Use either ComfygOptions.UseEncryption or ComfygOptions.UseKeyVault to configure secret handling.";

    internal string? AzureTableStorageConnectionString { get; private set; }

    internal string? EncryptionKey { get; private set; }

    internal Uri? KeyVaultUri { get; private set; }

    internal ComfygOptions()
    {
    }

    public ComfygOptions UseAzureTableStorage(string connectionString)
    {
        AzureTableStorageConnectionString =
            connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        return this;
    }

    public ComfygOptions UseEncryption(string encryptionKey)
    {
        if (KeyVaultUri != null) throw new InvalidOperationException(UseEitherEncryptionOrKeyVaultExceptionMessage);
        EncryptionKey = encryptionKey ?? throw new ArgumentNullException(nameof(encryptionKey));
        return this;
    }

    public ComfygOptions UseAzureKeyVault(string vaultUri)
    {
        if (EncryptionKey != null) throw new InvalidOperationException(UseEitherEncryptionOrKeyVaultExceptionMessage);
        if (vaultUri == null) throw new ArgumentNullException(nameof(vaultUri));
        KeyVaultUri = new Uri(vaultUri);
        return this;
    }
}
