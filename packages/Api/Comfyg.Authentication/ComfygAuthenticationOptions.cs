namespace Comfyg.Authentication;

public sealed class ComfygAuthenticationOptions
{
    private const string UseEitherEncryptionOrKeyVaultExceptionMessage =
        "Cannot use encryption and Azure Key Vault. Use either ComfygAuthenticationOptions.UseEncryption or ComfygAuthenticationOptions.UseKeyVault to configure secret handling.";

    internal string? AzureTableStorageConnectionString { get; private set; }

    internal string? EncryptionKey { get; private set; }

    internal bool UseKeyVault { get; private set; }

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
        if (UseKeyVault) throw new InvalidOperationException(UseEitherEncryptionOrKeyVaultExceptionMessage);
        EncryptionKey = encryptionKey ?? throw new ArgumentNullException(nameof(encryptionKey));
        return this;
    }

    //TODO support provide SecretClient option
    public ComfygAuthenticationOptions UseAzureKeyVault()
    {
        if (EncryptionKey != null) throw new InvalidOperationException(UseEitherEncryptionOrKeyVaultExceptionMessage);
        UseKeyVault = true;
        return this;
    }
}
