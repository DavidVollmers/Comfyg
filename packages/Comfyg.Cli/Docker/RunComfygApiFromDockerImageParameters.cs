namespace Comfyg.Cli.Docker;

internal class RunComfygApiFromDockerImageParameters
{
    public string Image { get; set; } = null!;

    public string SystemClientId { get; set; } = null!;

    public string SystemClientSecret { get; set; } = null!;

    public string SystemEncryptionKey { get; set; } = null!;

    public string SystemAzureTableStorageConnectionString { get; set; } = null!;

    public string AuthenticationEncryptionKey { get; set; } = null!;

    public string AuthenticationAzureTableStorageConnectionString { get; set; } = null!;

    public string AuthenticationAzureBlobStorageConnectionString { get; set; } = null!;
}
