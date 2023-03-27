using Comfyg.Client;
using Comfyg.Store.Contracts.Configuration;
using Comfyg.Store.Contracts.Requests;

namespace Comfyg.Cli.Commands.Import;

public class ImportConfigurationCommand : ImportCommandBase<IConfigurationValue>
{
    public ImportConfigurationCommand()
        : base("config", "Imports key-value pairs as configuration values into the connected Comfyg store.")
    {
    }

    protected override AddValuesRequest<IConfigurationValue> BuildAddValuesRequest(
        IEnumerable<KeyValuePair<string, string>> kvp)
    {
        return new AddConfigurationValuesRequest { Values = kvp.Select(i => new ConfigurationValue(i.Key, i.Value)) };
    }
}
