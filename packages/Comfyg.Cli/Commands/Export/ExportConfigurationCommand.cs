using Comfyg.Store.Contracts;

namespace Comfyg.Cli.Commands.Export;

internal class ExportConfigurationCommand : ExportCommandBase<IConfigurationValue>
{
    public ExportConfigurationCommand()
        : base("config", "Exports key-value pairs from the configuration values of the connected Comfyg store.")
    {
    }
}
