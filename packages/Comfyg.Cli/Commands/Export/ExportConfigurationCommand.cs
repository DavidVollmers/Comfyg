using Comfyg.Contracts.Configuration;

namespace Comfyg.Cli.Commands.Export;

public class ExportConfigurationCommand : ExportCommandBase<IConfigurationValue>
{
    public ExportConfigurationCommand()
        : base("config", "Exports configuration values to a JSON file")
    {
    }
}
