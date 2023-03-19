using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using Docker.DotNet;
using Docker.DotNet.Models;
using ICSharpCode.SharpZipLib.Tar;
using Spectre.Console;

namespace Comfyg.Cli.Commands.Setup;

public class SetupLocalhostCommand : Command
{
    private const string DockerImageLocalBuildTag = "comfyg-local-build";
    
    private readonly Option<FileInfo?> _dockerFileOption;
    private readonly Option<Uri?> _dockerSocketOption;

    public SetupLocalhostCommand() : base("localhost", "Setup a new Comfyg API running on your localhost")
    {
        _dockerFileOption = new Option<FileInfo?>(new[]
        {
            "-df",
            "--docker-file"
        }, "The Dockerfile to use for building the Comfyg API");
        AddOption(_dockerFileOption);

        _dockerSocketOption = new Option<Uri?>(new[]
        {
            "-ds",
            "--docker-socket"
        }, "The URI to the docker socket to use");
        AddOption(_dockerSocketOption);

        this.SetHandler(HandleCommandAsync);
    }

    private async Task HandleCommandAsync(InvocationContext context)
    {
        var dockerFileOption = context.ParseResult.GetValueForOption(_dockerFileOption);
        var dockerSocketOption = context.ParseResult.GetValueForOption(_dockerSocketOption);

        var cancellationToken = context.GetCancellationToken();

        var dockerConfiguration = dockerSocketOption == null
            ? new DockerClientConfiguration()
            : new DockerClientConfiguration(dockerSocketOption);

        using var dockerClient = dockerConfiguration.CreateClient();

        if (dockerFileOption != null)
        {
            if (!dockerFileOption.Exists)
                throw new FileNotFoundException("Could not find Dockerfile.", dockerFileOption.FullName);

            AnsiConsole.WriteLine("Building Docker Image...");
            var progress = new Text("Starting Dockerfile build...");
            await AnsiConsole.Live(progress).StartAsync(async displayContext =>
            {
                void MessageHandler(string message)
                {
                    displayContext.UpdateTarget(new Text(message));
                    displayContext.Refresh();
                }

                // ReSharper disable once AccessToDisposedClosure
                await BuildDockerfileAsync(dockerClient, dockerFileOption, MessageHandler, cancellationToken)
                    .ConfigureAwait(false);
            }).ConfigureAwait(false);
        }
        else
        {
            //TODO use docker registry to pull image
            throw new NotImplementedException();
        }
    }

    private static async Task BuildDockerfileAsync(IDockerClient dockerClient, FileInfo dockerFile,
        Action<string> messageHandler, CancellationToken cancellationToken)
    {
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
                    DockerImageLocalBuildTag
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