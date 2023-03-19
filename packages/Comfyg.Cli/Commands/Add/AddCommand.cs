using System.CommandLine;

namespace Comfyg.Cli.Commands.Add;

internal class AddCommand : Command
{
    // ReSharper disable SuggestBaseTypeForParameterInConstructor
    public AddCommand(AddConfigurationCommand addConfigurationCommand, AddSettingCommand addSettingCommand,
        AddSecretCommand addSecretCommand)
        : base("add", "Adds a Comfyg value")
    {
        if (addConfigurationCommand == null) throw new ArgumentNullException(nameof(addConfigurationCommand));
        if (addSettingCommand == null) throw new ArgumentNullException(nameof(addSettingCommand));
        if (addSecretCommand == null) throw new ArgumentNullException(nameof(addSecretCommand));

        AddCommand(addConfigurationCommand);
        AddCommand(addSettingCommand);
        AddCommand(addSecretCommand);
    }
    // ReSharper enable SuggestBaseTypeForParameterInConstructor
}