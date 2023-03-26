using Comfyg.Store.Contracts.Secrets;

namespace Comfyg.Cli.Commands.Export;

public class ExportSecretsCommand : ExportCommandBase<ISecretValue>
{
    public ExportSecretsCommand()
        : base("secrets", "Exports secret values to a JSON file")
    {
    }
}
