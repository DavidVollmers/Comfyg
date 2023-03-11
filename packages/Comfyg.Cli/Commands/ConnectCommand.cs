using System.CommandLine;
using System.CommandLine.Invocation;
using Comfyg.Client;

namespace Comfyg.Cli.Commands;

public class ConnectCommand : Command
{
    private readonly Option<string> _connectionStringOption;

    public ConnectCommand() : base("connect", "Connect to a Comfyg endpoint")
    {
        _connectionStringOption = new Option<string>(new[]
        {
            "-cs",
            "--connection-string"
        }, "The connection string which contains the information how to connect to the Comfyg endpoint")
        {
            IsRequired = true
        };
        AddOption(_connectionStringOption);

        this.SetHandler(HandleCommandAsync);
    }

    private async Task<int> HandleCommandAsync(InvocationContext context)
    {
        var connectionString = context.ParseResult.GetValueForOption(_connectionStringOption)!;

        using var client = new ComfygClient(connectionString);

        var cancellationToken = context.GetCancellationToken();

        await client.EstablishConnectionAsync(cancellationToken).ConfigureAwait(false);

        return 0;
    }
}