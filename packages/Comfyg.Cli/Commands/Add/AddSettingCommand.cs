using Comfyg.Client;
using Comfyg.Store.Contracts;

namespace Comfyg.Cli.Commands.Add;

internal class AddSettingCommand : AddValueCommandBase<ISettingValue>
{
    public AddSettingCommand() : base("setting",
        "Adds a key-value pair as a setting value to the connected Comfyg store.")
    {
    }

    protected override ISettingValue BuildValue(string key, string value)
    {
        return new SettingValue(key, value);
    }
}
