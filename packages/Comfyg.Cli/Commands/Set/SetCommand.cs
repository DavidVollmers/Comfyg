using System.CommandLine;

namespace Comfyg.Cli.Commands.Set;

internal class SetCommand : Command
{
    public SetCommand(SetPermissionsCommand setPermissionsCommand) : base("set", "Sets Comfyg resources.")
    {
        if (setPermissionsCommand == null) throw new ArgumentNullException(nameof(setPermissionsCommand));
        
        AddCommand(setPermissionsCommand);
    }
}
