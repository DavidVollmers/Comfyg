using Comfyg.Store.Contracts;

namespace Comfyg.Cli.Commands.Set;

internal class SetSecretPermissionsCommand : SetValuePermissionsCommandBase<ISecretValue>
{
    public SetSecretPermissionsCommand() : base("secret",
        "Sets the permission on a secret key-value pair for another client.")
    {
    }
}
