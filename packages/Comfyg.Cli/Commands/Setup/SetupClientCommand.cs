using System.CommandLine;
using System.CommandLine.Invocation;
using Comfyg.Cli.Extensions;
using Comfyg.Contracts.Requests;
using Spectre.Console;

namespace Comfyg.Cli.Commands.Setup;

internal class SetupClientCommand : Command
{
    private readonly Argument<string> _clientIdArgument;
    private readonly Argument<string> _friendlyNameArgument;

    public SetupClientCommand() : base("client", "Creates a new client on the connected Comfyg endpoint")
    {
        _clientIdArgument = new Argument<string>("client-id", "The identifier of the client");
        AddArgument(_clientIdArgument);

        _friendlyNameArgument = new Argument<string>("friendly-name", "The user friendly name of the client");
        AddArgument(_friendlyNameArgument);

        this.SetHandler(HandleCommandAsync);
    }

    private async Task HandleCommandAsync(InvocationContext context)
    {
        var clientIdArgument = context.ParseResult.GetValueForArgument(_clientIdArgument);
        var friendlyNameArgument = context.ParseResult.GetValueForArgument(_friendlyNameArgument);

        var cancellationToken = context.GetCancellationToken();

        using var client = await State.User.RequireClientAsync(cancellationToken).ConfigureAwait(false);

        var result = await client.SetupClientAsync(new SetupClientRequest
        {
            Client = new Client.Client(clientIdArgument, friendlyNameArgument)
        }, cancellationToken).ConfigureAwait(false);

        AnsiConsole.MarkupLine($"[bold green]Successfully created a client for {client.EndpointUrl}[/]");

        AnsiConsole.Write(result.Client.ToTable(result.ClientSecret));

        AnsiConsole.MarkupLine("[bold yellow]Make sure to copy the client secret before closing the terminal![/]");

        AnsiConsole.WriteLine("You can connect with this client using the following connection string:");
        AnsiConsole.MarkupLine(
            $"[bold]Endpoint={client.EndpointUrl};ClientId={result.Client.ClientId};ClientSecret={result.Client.ClientSecret};[/]");
    }
}