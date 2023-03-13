using System.CommandLine;
using System.CommandLine.Invocation;
using Comfyg.Client;

namespace Comfyg.Cli.Commands;

public class ConnectCommand : Command
{
    private readonly Argument<string> _connectionStringArgument;

    public ConnectCommand() : base("connect", "Connect to a Comfyg endpoint")
    {
        _connectionStringArgument = new Argument<string>("connection-string",
            "The connection string which contains the information on how to connect to the Comfyg endpoint");
        AddArgument(_connectionStringArgument);

        this.SetHandler(HandleCommandAsync);
    }

    private async Task HandleCommandAsync(InvocationContext context)
    {
        var connectionString = context.ParseResult.GetValueForArgument(_connectionStringArgument);

        using var client = new ComfygClient(connectionString);

        var cancellationToken = context.GetCancellationToken();

        await client.EstablishConnectionAsync(cancellationToken).ConfigureAwait(false);
    }
}