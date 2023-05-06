using System.CommandLine;

namespace Comfyg.Cli.Commands.Export;

internal class ExportCommand : Command
{
    // ReSharper disable SuggestBaseTypeForParameterInConstructor
    public ExportCommand(ExportConfigurationCommand exportConfigurationCommand,
        ExportSecretsCommand exportSecretsCommand, ExportSettingsCommand exportSettingsCommand)
        : base("export", "Exports key-value pairs from the connected Comfyg store.")
    {
        if (exportConfigurationCommand == null) throw new ArgumentNullException(nameof(exportConfigurationCommand));
        if (exportSecretsCommand == null) throw new ArgumentNullException(nameof(exportSecretsCommand));
        if (exportSettingsCommand == null) throw new ArgumentNullException(nameof(exportSettingsCommand));

        AddCommand(exportConfigurationCommand);
        AddCommand(exportSecretsCommand);
        AddCommand(exportSettingsCommand);
    }
    // ReSharper enable SuggestBaseTypeForParameterInConstructor
}
