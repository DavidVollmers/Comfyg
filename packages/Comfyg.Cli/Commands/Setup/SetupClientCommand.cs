using System.CommandLine;

namespace Comfyg.Cli.Commands.Setup;

public class SetupClientCommand : Command
{
    public SetupClientCommand() : base("client", "Creates a new client on the connected Comfyg endpoint")
    {
        
    }
}