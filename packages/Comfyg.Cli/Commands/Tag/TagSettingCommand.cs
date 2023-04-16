using Comfyg.Store.Contracts;

namespace Comfyg.Cli.Commands.Tag;

internal class TagSettingCommand : TagValueCommandBase<ISettingValue>
{
    public TagSettingCommand() : base("setting", "Tags a setting key-value pair.")
    {
    }
}
