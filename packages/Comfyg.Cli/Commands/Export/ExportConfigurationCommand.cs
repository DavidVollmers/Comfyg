using Comfyg.Store.Contracts.Configuration;

namespace Comfyg.Cli.Commands.Export;

public class ExportConfigurationCommand : ExportCommandBase<IConfigurationValue>
{
    public ExportConfigurationCommand()
        : base("config", "Exports key-value pairs from the configuration values of the connected Comfyg store.")
    {
    }
}
