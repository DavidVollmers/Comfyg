using Comfyg.Client;
using Comfyg.Store.Contracts.Requests;
using Comfyg.Store.Contracts.Secrets;

namespace Comfyg.Cli.Commands.Import;

internal class ImportSecretsCommand : ImportCommandBase<ISecretValue>
{
    public ImportSecretsCommand()
        : base("secrets", "Imports key-value pairs as secret values into the connected Comfyg store.")
    {
    }

    protected override AddValuesRequest<ISecretValue> BuildAddValuesRequest(
        IEnumerable<KeyValuePair<string, string>> kvp)
    {
        return new AddSecretValuesRequest { Values = kvp.Select(i => new SecretValue(i.Key, i.Value)) };
    }
}
