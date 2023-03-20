using System.CommandLine;
using System.CommandLine.Invocation;
using Comfyg.Cli.Docker;
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
    private readonly Option<string> _systemClientIdOption;
    private readonly Option<string> _systemClientSecretOption;
    private readonly Option<string> _systemEncryptionKeyOption;
    private readonly Option<string> _systemAzureTableStorageConnectionStringOption;
    private readonly Option<string> _authenticationEncryptionKeyOption;
    private readonly Option<string> _authenticationAzureTableStorageConnectionStringOption;

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

        _systemClientIdOption = new Option<string>(new[]
        {
            "-sci",
            "--system-client-id"
        }, "The ID of the Comfyg system client");
        AddOption(_systemClientIdOption);

        _systemClientSecretOption = new Option<string>(new[]
        {
            "-scs",
            "--system-client-secret"
        }, "The base64 secret of the Comfyg system client");
        AddOption(_systemClientSecretOption);

        _systemEncryptionKeyOption = new Option<string>(new[]
        {
            "-sek",
            "--system-encryption-key"
        }, "The base64 key used to encrypt all Comfyg system secrets");
        AddOption(_systemEncryptionKeyOption);

        _systemAzureTableStorageConnectionStringOption = new Option<string>(new[]
        {
            "-satscs",
            "--system-azure-table-storage-connection-string"
        }, "The connection string for the Azure table storage used to store all Comfyg system values");
        AddOption(_systemAzureTableStorageConnectionStringOption);

        _authenticationEncryptionKeyOption = new Option<string>(new[]
        {
            "-aek",
            "--authentication-encryption-key"
        }, "The base64 key used to encrypt all Comfyg authentication secrets");
        AddOption(_authenticationEncryptionKeyOption);

        _authenticationAzureTableStorageConnectionStringOption = new Option<string>(new[]
        {
            "-aatscs",
            "--authentication-azure-table-storage-connection-string"
        }, "The connection string for the Azure table storage used to store all Comfyg authentication values");
        AddOption(_authenticationAzureTableStorageConnectionStringOption);

        this.SetHandler(HandleCommandAsync);
    }

    private async Task HandleCommandAsync(InvocationContext context)
    {
        var dockerFileOption = context.ParseResult.GetValueForOption(_dockerFileOption);
        var dockerSocketOption = context.ParseResult.GetValueForOption(_dockerSocketOption);
        var systemClientIdOption = context.ParseResult.GetValueForOption(_systemClientIdOption);
        var systemClientSecretOption = context.ParseResult.GetValueForOption(_systemClientSecretOption);
        var systemEncryptionKeyOption = context.ParseResult.GetValueForOption(_systemEncryptionKeyOption);
        var systemAzureTableStorageConnectionStringOption =
            context.ParseResult.GetValueForOption(_systemAzureTableStorageConnectionStringOption);
        var authenticationEncryptionKeyOption =
            context.ParseResult.GetValueForOption(_authenticationEncryptionKeyOption);
        var authenticationAzureTableStorageConnectionStringOption =
            context.ParseResult.GetValueForOption(_authenticationAzureTableStorageConnectionStringOption);

        var cancellationToken = context.GetCancellationToken();

        var parameters = new RunComfygApiFromDockerImageParameters
        {
            SystemClientId = systemClientIdOption!,
            SystemClientSecret = systemClientSecretOption!,
            SystemEncryptionKey = systemEncryptionKeyOption!,
            SystemAzureTableStorageConnectionString = systemAzureTableStorageConnectionStringOption!,
            AuthenticationEncryptionKey = authenticationEncryptionKeyOption!,
            AuthenticationAzureTableStorageConnectionString = authenticationAzureTableStorageConnectionStringOption!
        };

        //TODO prompt missing parameters (support automatic generation)

        while (string.IsNullOrWhiteSpace(parameters.SystemClientId))
        {
            parameters.SystemClientId = AnsiConsole.Ask<string>("[bold]System Client ID[/]:");
        }

        if (!ValidateClientSecret(parameters.SystemClientSecret)) parameters.SystemClientSecret = null!;
        while (string.IsNullOrWhiteSpace(parameters.SystemClientSecret))
        {
            parameters.SystemClientSecret = AnsiConsole.Ask<string>("[bold]System Client Secret[/]:");
            
            if (!ValidateClientSecret(parameters.SystemClientSecret)) parameters.SystemClientSecret = null!;
        }

        RunComfygApiFromDockerImageResult result = null!;
        
        AnsiConsole.Clear();
        
        await AnsiConsole
            .Live(new Text("Initializing Setup"))
            .StartAsync(async ctx =>
            {
                var dockerConfiguration = dockerSocketOption == null
                    ? new DockerClientConfiguration()
                    : new DockerClientConfiguration(dockerSocketOption);

                using var dockerClient = dockerConfiguration.CreateClient();

                void MessageHandler(string message)
                {
                    ctx.UpdateTarget(new Text(message));
                }

                var existingContainers = await State.User
                    .ReadAsync<List<string>>(nameof(Docker), "Containers", cancellationToken)
                    .ConfigureAwait(false);
                if (existingContainers != null && existingContainers.Any())
                {
                    ctx.UpdateTarget(new Markup("[bold yellow]Removing existing Comfyg API Containers...[/]"));

                    foreach (var existingContainerId in existingContainers)
                    {
                        ctx.UpdateTarget(new Markup(
                            $"[bold yellow]Removing existing Comfyg API Container: {existingContainerId}[/]"));
                        
                        await dockerClient.Containers
                            .KillContainerAsync(existingContainerId, new ContainerKillParameters(), cancellationToken)
                            .ConfigureAwait(false);

                        await dockerClient.Containers.RemoveContainerAsync(existingContainerId,
                            new ContainerRemoveParameters
                            {
                                Force = true
                            }, cancellationToken).ConfigureAwait(false);

                        ctx.UpdateTarget(
                            new Markup($"Removed existing Comfyg API Container: [bold]{existingContainerId}[/]"));
                    }
                }

                await State.User.StoreAsync(nameof(Docker), "Containers", Array.Empty<string>(), cancellationToken)
                    .ConfigureAwait(false);

                if (dockerFileOption != null)
                {
                    if (!dockerFileOption.Exists)
                        throw new FileNotFoundException("Could not find Dockerfile.", dockerFileOption.FullName);

                    parameters.Image = DockerImageLocalBuildTag;

                    await dockerClient
                        .BuildImageFromDockerfileAsync(dockerFileOption, parameters.Image, MessageHandler,
                            cancellationToken)
                        .ConfigureAwait(false);
                }
                else
                {
                    //TODO use docker registry to pull image
                    throw new NotImplementedException();
                }

                result = await dockerClient
                    .RunComfygApiFromDockerImageAsync(parameters, MessageHandler, cancellationToken)
                    .ConfigureAwait(false);

                var containers = await State.User
                    .ReadAsync<List<string>>(nameof(Docker), "Containers", cancellationToken)
                    .ConfigureAwait(false) ?? new List<string>();
                containers.Add(result.ContainerId);

                await State.User.StoreAsync(nameof(Docker), "Containers", containers, cancellationToken)
                    .ConfigureAwait(false);

                ctx.UpdateTarget(new Markup("[bold green]Successfully started Comfyg API![/]"));
            }).ConfigureAwait(false);
        
        AnsiConsole.WriteLine("You can connect to your local Comfyg API using the following connection string:");
        AnsiConsole.MarkupLine(
            $"[bold]Endpoint=http://localhost:{result.Port};ClientId={parameters.SystemClientId};ClientSecret={parameters.SystemClientSecret};[/]");
    }

    private static bool ValidateClientSecret(string? clientSecret)
    {
        if (clientSecret == null) return false;
        
        try
        {
            var bytes = Convert.FromBase64String(clientSecret);
            if (bytes.Length >= 16) return true;
            
            AnsiConsole.WriteLine("Client secrets must be at least 16 bytes long.");
            return false;
        }
        catch (FormatException)
        {
            AnsiConsole.WriteLine("Client secrets must be base64 encoded.");
            return false;
        }
        catch
        {
            return false;
        }
    }
}