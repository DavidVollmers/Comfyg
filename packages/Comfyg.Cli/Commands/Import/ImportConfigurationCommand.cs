using Comfyg.Client;
using Comfyg.Store.Contracts.Configuration;
using Comfyg.Store.Contracts.Requests;

namespace Comfyg.Cli.Commands.Import;

public class ImportConfigurationCommand : ImportCommandBase<IConfigurationValue>
{
    public ImportConfigurationCommand()
        : base("config", "Imports comfyg values as configuration into the connected Comfyg endpoint")
    {
    }

    protected override AddValuesRequest<IConfigurationValue> BuildAddValuesRequest(
        IEnumerable<KeyValuePair<string, string>> kvp)
    {
        return new AddConfigurationValuesRequest { Values = kvp.Select(i => new ConfigurationValue(i.Key, i.Value)) };
    }
}
