using System.CommandLine;

namespace Comfyg.Cli.Commands.Tag;

internal class TagCommand : Command
{
    public TagCommand(TagConfigurationCommand tagConfigurationCommand, TagSecretCommand tagSecretCommand,
        TagSettingCommand tagSettingCommand) : base("tag", "Tags a key-valur pair version.")
    {
        if (tagConfigurationCommand == null) throw new ArgumentNullException(nameof(tagConfigurationCommand));
        if (tagSecretCommand == null) throw new ArgumentNullException(nameof(tagSecretCommand));
        if (tagSettingCommand == null) throw new ArgumentNullException(nameof(tagSettingCommand));
        
        AddCommand(tagConfigurationCommand);
        AddCommand(tagSecretCommand);
        AddCommand(tagSettingCommand);
    }
}
