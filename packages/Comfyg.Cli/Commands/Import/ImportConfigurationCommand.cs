using Comfyg.Client;
using Comfyg.Store.Contracts;

namespace Comfyg.Cli.Commands.Import;

internal class ImportConfigurationCommand : ImportCommandBase<IConfigurationValue>
{
    public ImportConfigurationCommand()
        : base("config", "Imports key-value pairs as configuration values into the connected Comfyg store.")
    {
    }

    protected override IEnumerable<IConfigurationValue> BuildAddValuesRequest(
        IEnumerable<KeyValuePair<string, string>> kvp)
    {
        return kvp.Select(i => new ConfigurationValue(i.Key, i.Value));
    }
}
