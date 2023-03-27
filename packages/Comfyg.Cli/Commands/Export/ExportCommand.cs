using System.CommandLine;

namespace Comfyg.Cli.Commands.Export;

public class ExportCommand : Command
{
    // ReSharper disable SuggestBaseTypeForParameterInConstructor
    public ExportCommand(ExportConfigurationCommand exportConfigurationCommand)
        : base("export", "Exports key-value pairs from the connected Comfyg store.")
    {
        if (exportConfigurationCommand == null) throw new ArgumentNullException(nameof(exportConfigurationCommand));

        AddCommand(exportConfigurationCommand);
    }
    // ReSharper enable SuggestBaseTypeForParameterInConstructor
}
