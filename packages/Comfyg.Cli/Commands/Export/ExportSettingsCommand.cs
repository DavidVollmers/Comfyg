using Comfyg.Contracts.Settings;

namespace Comfyg.Cli.Commands.Export;

public class ExportSettingsCommand : ExportCommandBase<ISettingValue>
{
    public ExportSettingsCommand()
        : base("settings", "Exports setting values to a JSON file")
    {
    }
}
