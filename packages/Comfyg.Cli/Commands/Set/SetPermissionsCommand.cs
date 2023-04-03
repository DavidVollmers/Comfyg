using System.CommandLine;

namespace Comfyg.Cli.Commands.Set;

internal class SetPermissionsCommand : Command
{
    public SetPermissionsCommand() : base("permissions", "Sets the permissions of the connected client for a different client.")
    {
    }
}
