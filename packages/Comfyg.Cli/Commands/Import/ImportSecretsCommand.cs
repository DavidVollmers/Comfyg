using Comfyg.Client;
using Comfyg.Store.Contracts.Requests;
using Comfyg.Store.Contracts.Secrets;

namespace Comfyg.Cli.Commands.Import;

public class ImportSecretsCommand : ImportCommandBase<ISecretValue>
{
    public ImportSecretsCommand()
        : base("secrets", "Imports comfyg values as secrets into the connected Comfyg endpoint")
    {
    }

    protected override AddValuesRequest<ISecretValue> BuildAddValuesRequest(
        IEnumerable<KeyValuePair<string, string>> kvp)
    {
        return new AddSecretValuesRequest { Values = kvp.Select(i => new SecretValue(i.Key, i.Value)) };
    }
}
