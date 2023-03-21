using System.CommandLine;
using System.CommandLine.Invocation;
using System.Security.Cryptography;
using Azure.Data.Tables;
using Comfyg.Cli.Docker;
using Comfyg.Cli.Extensions;
using Docker.DotNet;
using Docker.DotNet.Models;
using Spectre.Console;

namespace Comfyg.Cli.Commands.Setup;

internal class SetupLocalhostCommand : Command
{
    private const string DockerImageLocalBuildTag = "comfyg-local-build";
    private const string DockerImagePublic = "dvol/comfyg";

    private readonly Option<FileInfo?> _dockerFileOption;
    private readonly Option<Uri?> _dockerSocketOption;
    private readonly Option<string> _systemClientIdOption;
    private readonly Option<string> _systemClientSecretOption;
    private readonly Option<string> _systemEncryptionKeyOption;
    private readonly Option<string> _systemAzureTableStorageConnectionStringOption;
    private readonly Option<string> _authenticationEncryptionKeyOption;
    private readonly Option<string> _authenticationAzureTableStorageConnectionStringOption;
    private readonly Option<bool> _leaveContainerOnErrorOption;
    private readonly Option<string> _versionOption;

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

        _leaveContainerOnErrorOption = new Option<bool>(new[]
        {
            "-lcoe",
            "--leave-container-on-error"
        }, "Do not stop/remove the Docker Container when there is an error");
        AddOption(_leaveContainerOnErrorOption);

        _versionOption = new Option<string>(new[]
        {
            "-v",
            "--version"
        }, "The version of the Comfyg API to use");
        _versionOption.SetDefaultValue("latest");
        AddOption(_versionOption);

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
        var leaveContainerOnErrorOption = context.ParseResult.GetValueForOption(_leaveContainerOnErrorOption);
        var versionOption = context.ParseResult.GetValueForOption(_versionOption);

        var cancellationToken = context.GetCancellationToken();

        var parameters = new RunComfygApiFromDockerImageParameters
        {
            SystemClientId = systemClientIdOption!,
            SystemClientSecret = systemClientSecretOption!,
            SystemEncryptionKey = systemEncryptionKeyOption!,
            SystemAzureTableStorageConnectionString = systemAzureTableStorageConnectionStringOption!,
            AuthenticationEncryptionKey = authenticationEncryptionKeyOption!,
            AuthenticationAzureTableStorageConnectionString = authenticationAzureTableStorageConnectionStringOption!,
            LeaveContainerOnError = leaveContainerOnErrorOption
        };

        await PromptMissingParametersAsync(parameters, cancellationToken).ConfigureAwait(false);

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

                    await dockerClient
                        .BuildImageFromDockerfileAsync(dockerFileOption, DockerImageLocalBuildTag, MessageHandler,
                            cancellationToken)
                        .ConfigureAwait(false);

                    parameters.Image = DockerImageLocalBuildTag;
                }
                else
                {
                    await dockerClient
                        .PullImageFromDockerHubAsync(DockerImagePublic, versionOption!, MessageHandler,
                            cancellationToken).ConfigureAwait(false);
                    
                    parameters.Image = DockerImagePublic + ":" + versionOption;
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

    private static async Task PromptMissingParametersAsync(RunComfygApiFromDockerImageParameters parameters,
        CancellationToken cancellationToken)
    {
        AnsiConsole.WriteLine("Before the setup please provide all necessary values to run your Comfyg API...");

        while (string.IsNullOrWhiteSpace(parameters.SystemClientId))
        {
            parameters.SystemClientId = AnsiConsole.Ask<string>("[bold]System Client ID[/]:");
        }

        while (!ValidateSecurityValue(parameters.SystemClientSecret, "Client secret"))
        {
            var generate = AnsiConsole.Prompt(new ConfirmationPrompt("Do you want to generate a new client secret?"));

            parameters.SystemClientSecret = generate
                ? Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
                : AnsiConsole.Ask<string>("[bold]System Client Secret[/]:");
        }

        while (!ValidateSecurityValue(parameters.SystemEncryptionKey, "Encryption key"))
        {
            var generate = AnsiConsole.Prompt(new ConfirmationPrompt("Do you want to generate a new encryption key?"));

            parameters.SystemEncryptionKey = generate
                ? Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                : AnsiConsole.Ask<string>("[bold]System Encryption Key[/]:");
        }

        while (!await ValidateAzureTableStorageConnectionStringAsync(
                   parameters.SystemAzureTableStorageConnectionString, cancellationToken).ConfigureAwait(false))
        {
            parameters.SystemAzureTableStorageConnectionString =
                AnsiConsole.Ask<string>("[bold]System Azure Table Storage[/]:");
        }

        while (!ValidateSecurityValue(parameters.AuthenticationEncryptionKey, "Encryption key"))
        {
            var generate = AnsiConsole.Prompt(new ConfirmationPrompt("Do you want to generate a new encryption key?"));

            parameters.AuthenticationEncryptionKey = generate
                ? Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                : AnsiConsole.Ask<string>("[bold]Authentication Encryption Key[/]:");
        }

        while (!await ValidateAzureTableStorageConnectionStringAsync(
                   parameters.AuthenticationAzureTableStorageConnectionString, cancellationToken).ConfigureAwait(false))
        {
            parameters.AuthenticationAzureTableStorageConnectionString =
                AnsiConsole.Ask<string>("[bold]Authentication Azure Table Storage[/]:");
        }
    }

    private static bool ValidateSecurityValue(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;

        try
        {
            var bytes = Convert.FromBase64String(value);
            if (bytes.Length >= 16) return true;

            AnsiConsole.MarkupLine($"[bold red]{name}s must be at least 16 bytes long.[/]");
            return false;
        }
        catch (FormatException)
        {
            AnsiConsole.MarkupLine($"[bold red]{name}s must be base64 encoded.[/]");
            return false;
        }
        catch
        {
            return false;
        }
    }

    private static async Task<bool> ValidateAzureTableStorageConnectionStringAsync(string connectionString,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) return false;

        try
        {
            var serviceClient = new TableServiceClient(connectionString);

            await serviceClient.GetPropertiesAsync(cancellationToken).ConfigureAwait(false);

            return true;
        }
        catch
        {
            AnsiConsole.MarkupLine(
                "[bold red]Could not connect to Azure Table Storage. Please make sure that the connection string is valid.[/]");

            return false;
        }
    }
}