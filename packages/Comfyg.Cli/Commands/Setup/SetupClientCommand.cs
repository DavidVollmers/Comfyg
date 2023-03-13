using System.CommandLine;
using System.CommandLine.Invocation;

namespace Comfyg.Cli.Commands.Setup;

internal class SetupClientCommand : Command
{
    private readonly Argument<string> _clientIdArgument;

    public SetupClientCommand() : base("client", "Creates a new client on the connected Comfyg endpoint")
    {
        _clientIdArgument = new Argument<string>("client-id", "The identifier of the client");
        AddArgument(_clientIdArgument);
        
        this.SetHandler(HandleCommandAsync);
    }

    private async Task HandleCommandAsync(InvocationContext context)
    {
    }
}