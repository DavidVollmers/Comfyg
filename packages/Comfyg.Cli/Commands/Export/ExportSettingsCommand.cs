using Comfyg.Store.Contracts.Settings;

namespace Comfyg.Cli.Commands.Export;

internal class ExportSettingsCommand : ExportCommandBase<ISettingValue>
{
    public ExportSettingsCommand()
        : base("settings", "Exports setting key-value pairs from the setting values of the connected Comfyg store.")
    {
    }
}
