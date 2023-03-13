namespace Comfyg.Core;

public sealed class ComfygOptions
{
    internal string? AzureTableStorageConnectionString { get; private set; }

    internal ComfygOptions()
    {
    }

    public ComfygOptions UseAzureTableStorage(string connectionString)
    {
        AzureTableStorageConnectionString =
            connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        return this;
    }
}