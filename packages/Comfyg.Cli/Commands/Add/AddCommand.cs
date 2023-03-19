using System.CommandLine;

namespace Comfyg.Cli.Commands.Add;

public class AddCommand : Command
{
    // ReSharper disable SuggestBaseTypeForParameterInConstructor
    public AddCommand(AddConfigurationCommand addConfigurationCommand, AddSettingCommand addSettingCommand,
        AddSecretCommand addSecretCommand)
        : base("add", "Adds a Comfyg value")
    {
        if (addConfigurationCommand == null) throw new ArgumentNullException(nameof(addConfigurationCommand));

        AddCommand(addConfigurationCommand);
        AddCommand(addSettingCommand);
        AddCommand(addSecretCommand);
    }
    // ReSharper enable SuggestBaseTypeForParameterInConstructor
}