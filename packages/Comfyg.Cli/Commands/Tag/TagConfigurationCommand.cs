using Comfyg.Store.Contracts;

namespace Comfyg.Cli.Commands.Tag;

internal class TagConfigurationCommand : TagValueCommandBase<IConfigurationValue>
{
    public TagConfigurationCommand() : base("config", "Tags a configuration key-value pair.")
    {
    }
}
