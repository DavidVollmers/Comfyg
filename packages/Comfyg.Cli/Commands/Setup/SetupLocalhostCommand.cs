using System.CommandLine;
using System.CommandLine.Invocation;
using Comfyg.Cli.Extensions;
using Docker.DotNet;
using Docker.DotNet.Models;
using Spectre.Console;

namespace Comfyg.Cli.Commands.Setup;

internal class SetupLocalhostCommand : Command
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

        AnsiConsole.WriteLine();
        var progress = new Text("Initializing Setup...");
        await AnsiConsole.Live(progress).StartAsync(async displayContext =>
        {
            var dockerConfiguration = dockerSocketOption == null
                ? new DockerClientConfiguration()
                : new DockerClientConfiguration(dockerSocketOption);

            using var dockerClient = dockerConfiguration.CreateClient();

            void MessageHandler(string message)
            {
                displayContext.UpdateTarget(new Text(message));
            }

            var existingContainers = await State.User
                .ReadAsync<List<string>>(nameof(Docker), "Containers", cancellationToken)
                .ConfigureAwait(false);
            if (existingContainers != null && existingContainers.Any())
            {
                displayContext.UpdateTarget(
                    new Markup("[bold yellow]Removing existing Comfyg API Containers...[/]"));
                foreach (var existingContainerId in existingContainers)
                {
                    displayContext.UpdateTarget(
                        new Markup($"[bold yellow]Removing existing Comfyg API Container: {existingContainerId}[/]"));

                    await dockerClient.Containers
                        .StopContainerAsync(existingContainerId, new ContainerStopParameters(), cancellationToken)
                        .ConfigureAwait(false);

                    await dockerClient.Containers.RemoveContainerAsync(existingContainerId,
                        new ContainerRemoveParameters
                        {
                            Force = true
                        }, cancellationToken).ConfigureAwait(false);
                }
            }

            string image;
            if (dockerFileOption != null)
            {
                if (!dockerFileOption.Exists)
                    throw new FileNotFoundException("Could not find Dockerfile.", dockerFileOption.FullName);

                image = DockerImageLocalBuildTag;

                await dockerClient
                    .BuildImageFromDockerfileAsync(dockerFileOption, image, MessageHandler, cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                //TODO use docker registry to pull image
                throw new NotImplementedException();
            }

            var containerId = await dockerClient
                .RunComfygApiFromDockerImageAsync(image, MessageHandler, cancellationToken)
                .ConfigureAwait(false);

            var containers = await State.User.ReadAsync<List<string>>(nameof(Docker), "Containers", cancellationToken)
                .ConfigureAwait(false) ?? new List<string>();
            containers.Add(containerId);

            await State.User.StoreAsync(nameof(Docker), "Containers", containers, cancellationToken)
                .ConfigureAwait(false);

            displayContext.UpdateTarget(new Markup("[bold green]Successfully started Comfyg API![/]"));
        }).ConfigureAwait(false);
    }
}