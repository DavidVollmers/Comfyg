using Comfyg.Store.Contracts.Secrets;

namespace Comfyg.Cli.Commands.Export;

internal class ExportSecretsCommand : ExportCommandBase<ISecretValue>
{
    public ExportSecretsCommand()
        : base("secrets", "Exports secret key-value pairs from the secret values of the connected Comfyg store.")
    {
    }
}
