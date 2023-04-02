using System.CommandLine;
using System.CommandLine.Invocation;
using Comfyg.Cli.Extensions;
using Comfyg.Client;
using Spectre.Console;

namespace Comfyg.Cli.Commands;

internal class ConnectCommand : Command
{
    private readonly Argument<string> _connectionStringArgument;

    public ConnectCommand() : base("connect", "Establishes a connection to a Comfyg store.")
    {
        _connectionStringArgument = new Argument<string>("connection-string",
            "The connection string used to connect to the Comfyg store.");
        AddArgument(_connectionStringArgument);

        this.SetHandler(HandleCommandAsync);
    }

    private async Task HandleCommandAsync(InvocationContext context)
    {
        var connectionString = context.ParseResult.GetValueForArgument(_connectionStringArgument);

        using var client = new ComfygClient(connectionString);

        var cancellationToken = context.GetCancellationToken();

        var result = await client.EstablishConnectionAsync(cancellationToken);

        AnsiConsole.MarkupLine($"[bold green]Successfully connected to {client.EndpointUrl}[/]");

        AnsiConsole.Write(result.Client.ToTable());

        await State.User.StoreAsync(nameof(Comfyg), nameof(ComfygClient), connectionString, cancellationToken);
    }
}
