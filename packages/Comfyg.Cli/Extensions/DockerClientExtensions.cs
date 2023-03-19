using System.Text;
using Docker.DotNet;
using Docker.DotNet.Models;
using ICSharpCode.SharpZipLib.Tar;

namespace Comfyg.Cli.Extensions;

internal static class DockerClientExtensions
{
    public static async Task<string> RunComfygApiFromDockerImageAsync(this IDockerClient dockerClient, string image,
        Action<string> messageHandler, CancellationToken cancellationToken)
    {
        if (dockerClient == null) throw new ArgumentNullException(nameof(dockerClient));
        if (image == null) throw new ArgumentNullException(nameof(image));
        if (messageHandler == null) throw new ArgumentNullException(nameof(messageHandler));

        //TODO calculate free port
        const int hostPort80 = 3421;
        var environmentVariables = new List<string>();

        messageHandler("Creating Docker Container...");

        var response = await dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = image,
            Env = environmentVariables,
            HostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    {
                        80.ToString(), new List<PortBinding>
                        {
                            new() { HostPort = hostPort80.ToString() }
                        }
                    }
                }
            }
        }, cancellationToken).ConfigureAwait(false);

        messageHandler("Successfully created Docker Container: " + response.ID);

        try
        {
            var result = await dockerClient.Containers
                .StartContainerAsync(response.ID, new ContainerStartParameters(), cancellationToken)
                .ConfigureAwait(false);

            if (!result) throw new Exception("Could not start Docker Container!");
        }
        catch
        {
            messageHandler("Could not start Docker Container. Trying to remove it...");
            
            await dockerClient.Containers.RemoveContainerAsync(response.ID, new ContainerRemoveParameters
            {
                Force = true
            }, cancellationToken).ConfigureAwait(false);
            
            throw;
        }

        return response.ID;
    }

    public static async Task BuildImageFromDockerfileAsync(this IDockerClient dockerClient, FileInfo dockerFile,
        string image, Action<string> messageHandler, CancellationToken cancellationToken)
    {
        if (dockerClient == null) throw new ArgumentNullException(nameof(dockerClient));
        if (dockerFile == null) throw new ArgumentNullException(nameof(dockerFile));
        if (image == null) throw new ArgumentNullException(nameof(image));
        if (messageHandler == null) throw new ArgumentNullException(nameof(messageHandler));

        await using var stream =
            await CreateStreamFromDockerfileDirectoryAsync(dockerFile.Directory!, messageHandler, cancellationToken)
                .ConfigureAwait(false);

        var progress = new Progress<JSONMessage>();
        progress.ProgressChanged += (_, message) =>
        {
            if (message.Stream != null) messageHandler(message.Stream);
        };

        await dockerClient.Images.BuildImageFromDockerfileAsync(new ImageBuildParameters
            {
                Dockerfile = dockerFile.Name,
                Tags = new List<string>
                {
                    image
                }
            }, stream, Array.Empty<AuthConfig>(), new Dictionary<string, string>(), progress, cancellationToken)
            .ConfigureAwait(false);
    }

    private static async Task<Stream> CreateStreamFromDockerfileDirectoryAsync(FileSystemInfo directoryInfo,
        Action<string> messageHandler, CancellationToken cancellationToken)
    {
        messageHandler("Creating dockerfile build context...");

        var stream = new MemoryStream();

        await using var archive = new TarOutputStream(stream, Encoding.UTF8)
        {
            IsStreamOwner = false
        };

        var files = Directory.GetFiles(directoryInfo.FullName, "*.*", SearchOption.AllDirectories);
        messageHandler($"Found {files.Length} files");

        foreach (var file in files)
        {
            var name = file[directoryInfo.FullName.Length..].Replace('\\', '/').TrimStart('/');

            messageHandler($"Adding \"{name}\" to the build context...");

            var entry = TarEntry.CreateTarEntry(name);
            await using var fileStream = File.OpenRead(file);
            entry.Size = fileStream.Length;
            await archive.PutNextEntryAsync(entry, cancellationToken);

            int numRead;
            var buffer = new byte[32 * 1024];
            while ((numRead = await fileStream.ReadAsync(buffer, cancellationToken)) > 0)
            {
                await archive.WriteAsync(buffer.AsMemory(0, numRead), cancellationToken);
            }

            await archive.CloseEntryAsync(cancellationToken);
        }

        archive.Close();

        stream.Position = 0;
        return stream;
    }
}