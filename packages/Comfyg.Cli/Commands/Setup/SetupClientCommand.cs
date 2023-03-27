using System.CommandLine;
using System.CommandLine.Invocation;
using Comfyg.Cli.Extensions;
using Comfyg.Store.Contracts.Requests;
using Spectre.Console;

namespace Comfyg.Cli.Commands.Setup;

internal class SetupClientCommand : Command
{
    private readonly Argument<string> _clientIdArgument;
    private readonly Argument<string> _friendlyNameArgument;

    public SetupClientCommand() : base("client", "Registers a new client on the connected Comfyg store.")
    {
        _clientIdArgument = new Argument<string>("CLIENT_ID", "The ID of the client to create. This must be unique for the connected Comfyg store.");
        AddArgument(_clientIdArgument);

        _friendlyNameArgument = new Argument<string>("FRIENDLY_NAME", "The user friendly display name which is used for the created client.");
        AddArgument(_friendlyNameArgument);

        this.SetHandler(HandleCommandAsync);
    }

    private async Task HandleCommandAsync(InvocationContext context)
    {
        var clientIdArgument = context.ParseResult.GetValueForArgument(_clientIdArgument);
        var friendlyNameArgument = context.ParseResult.GetValueForArgument(_friendlyNameArgument);

        var cancellationToken = context.GetCancellationToken();

        using var client = await State.User.RequireClientAsync(cancellationToken);

        var result = await client.SetupClientAsync(new SetupClientRequest
        {
            Client = new Client.Client(clientIdArgument, friendlyNameArgument)
        }, cancellationToken);

        AnsiConsole.MarkupLine($"[bold green]Successfully created a client for {client.EndpointUrl}[/]");

        AnsiConsole.Write(result.Client.ToTable(result.ClientSecret));

        AnsiConsole.MarkupLine("[bold yellow]Make sure to copy the client secret before closing the terminal![/]");

        AnsiConsole.WriteLine("You can connect with this client using the following connection string:");
        AnsiConsole.MarkupLine(
            $"[bold]Endpoint={client.EndpointUrl};ClientId={result.Client.ClientId};ClientSecret={result.ClientSecret};[/]");
    }
}
