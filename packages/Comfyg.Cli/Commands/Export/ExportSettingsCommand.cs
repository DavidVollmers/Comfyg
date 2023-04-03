using Comfyg.Store.Contracts;

namespace Comfyg.Cli.Commands.Export;

internal class ExportSettingsCommand : ExportValuesCommandBase<ISettingValue>
{
    public ExportSettingsCommand()
        : base("settings", "Exports setting key-value pairs from the setting values of the connected Comfyg store.")
    {
    }
}
