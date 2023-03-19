namespace Comfyg.Cli.Docker;

internal class RunComfygApiFromDockerImageResult
{
    public string ContainerId { get; }

    public int Port { get; set; }

    public RunComfygApiFromDockerImageResult(string containerId, int port)
    {
        ContainerId = containerId;
        Port = port;
    }
}