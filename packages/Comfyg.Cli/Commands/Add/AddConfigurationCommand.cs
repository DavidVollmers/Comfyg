using Comfyg.Client;
using Comfyg.Store.Contracts;

namespace Comfyg.Cli.Commands.Add;

internal class AddConfigurationCommand : AddValueCommandBase<IConfigurationValue>
{
    public AddConfigurationCommand() : base("config",
        "Adds a key-value pair as a configuration value to the connected Comfyg store.")
    {
    }

    protected override IConfigurationValue BuildValue(string key, string value)
    {
        return new ConfigurationValue(key, value);
    }
}
