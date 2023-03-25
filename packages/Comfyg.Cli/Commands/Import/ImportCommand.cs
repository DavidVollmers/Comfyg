using System.CommandLine;

namespace Comfyg.Cli.Commands.Import;

internal class ImportCommand : Command
{
    // ReSharper disable SuggestBaseTypeForParameterInConstructor
    public ImportCommand(ImportConfigurationCommand importConfigurationCommand,
        ImportSettingsCommand importSettingsCommand, ImportSecretsCommand importSecretsCommand)
        : base("import", "Import Comfyg values")
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
