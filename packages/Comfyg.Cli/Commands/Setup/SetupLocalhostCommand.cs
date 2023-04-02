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
    private readonly Option<string> _versionOption;

    public SetupLocalhostCommand() : base("localhost", "Sets up a new Comfyg store on your local machine.")
    {
        _dockerFileOption = new Option<FileInfo?>(new[]
        {
            "-df",
            "--docker-file"
        }, "The Dockerfile used to build the container from. This can be used for local development of the Comfyg store API or when testing your own Comfyg store API implementation.");
        AddOption(_dockerFileOption);

        _dockerSocketOption = new Option<Uri?>(new[]
        {
            "-ds",
            "--docker-socket"
        }, "The docker socket to use for creating and running the docker container.");
        AddOption(_dockerSocketOption);

        _systemClientIdOption = new Option<string>(new[]
        {
            "--system-client-id"
        }, "The client ID of the Comfyg system client.");
        AddOption(_systemClientIdOption);

        _systemClientSecretOption = new Option<string>(new[]
        {
            "--system-client-secret"
        }, "The client secret of the Comfyg system client.");
        AddOption(_systemClientSecretOption);

        _systemEncryptionKeyOption = new Option<string>(new[]
        {
            "--system-encryption-key"
        }, "The base64 encoded key used to encrypt all Comfyg secret values.");
        AddOption(_systemEncryptionKeyOption);

        _systemAzureTableStorageConnectionStringOption = new Option<string>(new[]
        {
            "--system-azure-table-storage-connection-string"
        }, "A connection string to connect to the Azure Storage Account used to store all Comfyg values.");
        AddOption(_systemAzureTableStorageConnectionStringOption);

        _authenticationEncryptionKeyOption = new Option<string>(new[]
        {
            "--authentication-encryption-key"
        }, "The base64 encoded key used to encrypt all authentication related secrets.");
        AddOption(_authenticationEncryptionKeyOption);

        _authenticationAzureTableStorageConnectionStringOption = new Option<string>(new[]
        {
            "--authentication-azure-table-storage-connection-string"
        }, "A connection string to connect to the Azure Storage Account used to store all authentication related values.");
        AddOption(_authenticationAzureTableStorageConnectionStringOption);

        _versionOption = new Option<string>(new[]
        {
            "-v",
            "--version"
        }, "The version of the Docker Image to used. Defaults to `latest`.");
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
        var versionOption = context.ParseResult.GetValueForOption(_versionOption);

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

        await PromptMissingParametersAsync(parameters, cancellationToken);

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
                    .ReadAsync<List<string>>(nameof(Docker), "Containers", cancellationToken);
                if (existingContainers != null && existingContainers.Any())
                {
                    ctx.UpdateTarget(new Markup("[bold yellow]Removing existing Comfyg store Containers...[/]"));

                    foreach (var existingContainerId in existingContainers)
                    {
                        ctx.UpdateTarget(new Markup(
                            $"[bold yellow]Removing existing Comfyg store Container: {existingContainerId}[/]"));

                        await dockerClient.TryKillAndRemoveDockerContainerAsync(existingContainerId, cancellationToken);

                        ctx.UpdateTarget(
                            new Markup($"Removed existing Comfyg store Container: [bold]{existingContainerId}[/]"));
                    }
                }

                await State.User.StoreAsync(nameof(Docker), "Containers", Array.Empty<string>(), cancellationToken);

                if (dockerFileOption != null)
                {
                    if (!dockerFileOption.Exists)
                        throw new FileNotFoundException("Could not find Dockerfile.", dockerFileOption.FullName);

                    await dockerClient
                        .BuildImageFromDockerfileAsync(dockerFileOption, DockerImageLocalBuildTag, MessageHandler,
                            cancellationToken);

                    parameters.Image = DockerImageLocalBuildTag;
                }
                else
                {
                    await dockerClient
                        .PullImageFromDockerHubAsync(DockerImagePublic, versionOption!, MessageHandler,
                            cancellationToken);

                    parameters.Image = DockerImagePublic + ":" + versionOption;
                }

                result = await dockerClient
                    .RunComfygApiFromDockerImageAsync(parameters, MessageHandler, cancellationToken);

                var containers = await State.User
                    .ReadAsync<List<string>>(nameof(Docker), "Containers", cancellationToken)
                     ?? new List<string>();
                containers.Add(result.ContainerId);

                await State.User.StoreAsync(nameof(Docker), "Containers", containers, cancellationToken);

                ctx.UpdateTarget(new Markup("[bold green]Successfully started local Comfyg store[/]"));
            });

        AnsiConsole.WriteLine("You can connect to your local Comfyg store using the following connection string:");
        AnsiConsole.MarkupLine(
            $"[bold]Endpoint=http://localhost:{result.Port};ClientId={parameters.SystemClientId};ClientSecret={parameters.SystemClientSecret};[/]");
    }

    private static async Task PromptMissingParametersAsync(RunComfygApiFromDockerImageParameters parameters,
        CancellationToken cancellationToken)
    {
        AnsiConsole.WriteLine("Before the setup please provide all necessary values to run your Comfyg store...");

        while (string.IsNullOrWhiteSpace(parameters.SystemClientId))
        {
            parameters.SystemClientId = AnsiConsole.Ask<string>("[bold]System Client ID[/]:", "system");
        }

        while (!ValidateSecurityValue(parameters.SystemClientSecret, "Client secret"))
        {
            var generate =
                AnsiConsole.Prompt(
                    new ConfirmationPrompt("[bold]System Client Secret[/]: Generate a new client secret?"));

            parameters.SystemClientSecret = generate
                ? Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
                : AnsiConsole.Ask<string>("[bold]System Client Secret[/]:");
        }

        while (!ValidateSecurityValue(parameters.SystemEncryptionKey, "Encryption key"))
        {
            var generate =
                AnsiConsole.Prompt(
                    new ConfirmationPrompt("[bold]System Encryption Key[/]: Generate a new encryption key?"));

            parameters.SystemEncryptionKey = generate
                ? Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                : AnsiConsole.Ask<string>("[bold]System Encryption Key[/]:");
        }

        while (!await ValidateAzureTableStorageConnectionStringAsync(
                   parameters.SystemAzureTableStorageConnectionString, cancellationToken))
        {
            parameters.SystemAzureTableStorageConnectionString =
                AnsiConsole.Ask<string>("[bold]System Azure Table Storage[/]:");
        }

        while (!ValidateSecurityValue(parameters.AuthenticationEncryptionKey, "Encryption key"))
        {
            var generate =
                AnsiConsole.Prompt(
                    new ConfirmationPrompt("[bold]Authentication Encryption Key[/]: Generate a new encryption key?"));

            parameters.AuthenticationEncryptionKey = generate
                ? Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                : AnsiConsole.Ask<string>("[bold]Authentication Encryption Key[/]:");
        }

        while (!await ValidateAzureTableStorageConnectionStringAsync(
                   parameters.AuthenticationAzureTableStorageConnectionString, cancellationToken))
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

            await serviceClient.GetPropertiesAsync(cancellationToken);

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
