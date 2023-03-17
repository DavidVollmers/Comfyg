using System.CommandLine;
using System.CommandLine.Invocation;
using Docker.DotNet;
using Docker.DotNet.Models;
using Spectre.Console;

namespace Comfyg.Cli.Commands.Setup;

public class SetupLocalhostCommand : Command
{
    private readonly Option<FileInfo?> _dockerFileOption;
    private readonly Option<Uri?> _dockerSocketOption;

    public SetupLocalhostCommand() : base("localhost", "Setup a new Comfyg API running on your localhost")
    {
        _dockerFileOption = new Option<FileInfo?>(new[]
        {
            "-df",
            "--docker-file"
        }, "The Dockerfile to use for building the Comfyg API");

        _dockerSocketOption = new Option<Uri?>(new[]
        {
            "-ds",
            "--docker-socket"
        }, "The URI to the docker socket to use");

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

            var progress = new Markup("Starting Dockerfile build...");
            await AnsiConsole.Live(progress).StartAsync(async displayContext =>
            {
                void MessageHandler(object sender, JSONMessage message)
                {
                    if (message.ErrorMessage != null)
                    {
                        displayContext.UpdateTarget(new Markup($"[bold red]{message.ErrorMessage}[/]"));
                    }
                    else
                    {
                        displayContext.UpdateTarget(new Markup(message.ProgressMessage));
                    }

                    displayContext.Refresh();
                }

                // ReSharper disable once AccessToDisposedClosure
                await BuildDockerfileAsync(dockerClient, dockerFileOption, MessageHandler!, cancellationToken)
                    .ConfigureAwait(false);
            }).ConfigureAwait(false);
        }
        else
        {
            //TODO use docker registry to pull image
            throw new NotImplementedException();
        }
    }

    private static async Task BuildDockerfileAsync(IDockerClient dockerClient, FileSystemInfo dockerFile,
        EventHandler<JSONMessage> messageHandler, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();

        var progress = new Progress<JSONMessage>();
        progress.ProgressChanged += messageHandler;

        await dockerClient.Images.BuildImageFromDockerfileAsync(new ImageBuildParameters
            {
                Dockerfile = dockerFile.FullName
            }, stream, Array.Empty<AuthConfig>(), new Dictionary<string, string>(), progress, cancellationToken)
            .ConfigureAwait(false);
    }
}