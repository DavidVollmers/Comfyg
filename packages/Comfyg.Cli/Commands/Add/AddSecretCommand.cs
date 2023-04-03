using Comfyg.Client;
using Comfyg.Store.Contracts;

namespace Comfyg.Cli.Commands.Add;

internal class AddSecretCommand : AddValueCommandBase<ISecretValue>
{
    public AddSecretCommand() : base("secret", "Adds a key-value pair as a secret value to the connected Comfyg store.")
    {
    }

    protected override ISecretValue BuildValue(string key, string value)
    {
        return new SecretValue(key, value);
    }
}
