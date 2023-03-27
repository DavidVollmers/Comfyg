using Comfyg.Client;
using Comfyg.Store.Contracts.Requests;
using Comfyg.Store.Contracts.Settings;

namespace Comfyg.Cli.Commands.Import;

internal class ImportSettingsCommand : ImportCommandBase<ISettingValue>
{
    public ImportSettingsCommand()
        : base("settings", "Imports key-value pairs as setting values into the connected Comfyg store.")
    {
    }

    protected override AddValuesRequest<ISettingValue> BuildAddValuesRequest(
        IEnumerable<KeyValuePair<string, string>> kvp)
    {
        return new AddSettingValuesRequest { Values = kvp.Select(i => new SettingValue(i.Key, i.Value)) };
    }
}
