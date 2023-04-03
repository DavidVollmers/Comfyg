using Comfyg.Store.Contracts;

namespace Comfyg.Cli.Commands.Set;

internal class SetConfigurationPermissionsCommand : SetValuePermissionsCommandBase<IConfigurationValue>
{
    public SetConfigurationPermissionsCommand() : base("config",
        "Sets the permission on a configuration key-value pair for a different client.")
    {
    }
}
