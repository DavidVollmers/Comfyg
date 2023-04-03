using System.CommandLine;

namespace Comfyg.Cli.Commands.Setup;

internal class SetupCommand : Command
{
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    public SetupCommand(SetupClientCommand setupClientCommand, SetupLocalhostCommand setupLocalhostCommand) : base(
        "setup", "Setup Comfyg resources.")
    {
        if (setupClientCommand == null) throw new ArgumentNullException(nameof(setupClientCommand));
        if (setupLocalhostCommand == null) throw new ArgumentNullException(nameof(setupLocalhostCommand));

        AddCommand(setupClientCommand);
        AddCommand(setupLocalhostCommand);
    }
}
