using System.CommandLine;

namespace Comfyg.Cli.Commands.Add;

public class AddCommand : Command
{
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    public AddCommand(AddConfigurationCommand addConfigurationCommand) : base("add", "Adds a Comfyg value")
    {
        if (addConfigurationCommand == null) throw new ArgumentNullException(nameof(addConfigurationCommand));

        AddCommand(addConfigurationCommand);
    }
}