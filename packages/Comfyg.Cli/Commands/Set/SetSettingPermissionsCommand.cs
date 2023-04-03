using Comfyg.Store.Contracts;

namespace Comfyg.Cli.Commands.Set;

internal class SetSettingPermissionsCommand : SetValuePermissionsCommandBase<ISecretValue>
{
    public SetSettingPermissionsCommand() : base("setting",
        "Sets the permission on a setting key-value pair for a different client.")
    {
    }
}
