using System.CommandLine;

namespace Comfyg.Cli.Commands.Import;

internal class ImportCommand : Command
{
    // ReSharper disable SuggestBaseTypeForParameterInConstructor
    public ImportCommand(ImportConfigurationCommand importConfigurationCommand,
        ImportSettingsCommand importSettingsCommand, ImportSecretsCommand importSecretsCommand)
        : base("import", "Imports key-value pairs into the connected Comfyg store.")
    {
        if (importConfigurationCommand == null) throw new ArgumentNullException(nameof(importConfigurationCommand));
        if (importSettingsCommand == null) throw new ArgumentNullException(nameof(importSettingsCommand));
        if (importSecretsCommand == null) throw new ArgumentNullException(nameof(importSecretsCommand));

        AddCommand(importConfigurationCommand);
        AddCommand(importSettingsCommand);
        AddCommand(importSecretsCommand);
    }
    // ReSharper enable SuggestBaseTypeForParameterInConstructor
}
