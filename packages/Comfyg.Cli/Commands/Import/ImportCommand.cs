using System.CommandLine;

namespace Comfyg.Cli.Commands.Import;

internal class ImportCommand : Command
{
    // ReSharper disable SuggestBaseTypeForParameterInConstructor
    public ImportCommand(ImportConfigurationCommand importConfigurationCommand)
        : base("import", "Import Comfyg values")
    {
        if (importConfigurationCommand == null) throw new ArgumentNullException(nameof(importConfigurationCommand));

        AddCommand(importConfigurationCommand);
    }
    // ReSharper enable SuggestBaseTypeForParameterInConstructor
}
