using Comfyg.Client;
using Comfyg.Store.Contracts.Requests;
using Comfyg.Store.Contracts.Settings;

namespace Comfyg.Cli.Commands.Import;

public class ImportSettingsCommand : ImportCommandBase<ISettingValue>
{
    public ImportSettingsCommand()
        : base("settings", "Imports comfyg values as settings into the connected Comfyg endpoint")
    {
    }

    protected override AddValuesRequest<ISettingValue> BuildAddValuesRequest(
        IEnumerable<KeyValuePair<string, string>> kvp)
    {
        return new AddSettingValuesRequest { Values = kvp.Select(i => new SettingValue(i.Key, i.Value)) };
    }
}
