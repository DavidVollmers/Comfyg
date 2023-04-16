using Comfyg.Store.Contracts;

namespace Comfyg.Cli.Commands.Tag;

internal class TagSecretCommand : TagValueCommandBase<ISecretValue>
{
    public TagSecretCommand() : base("secret", "Tags a secret key-value pair.")
    {
    }
}
