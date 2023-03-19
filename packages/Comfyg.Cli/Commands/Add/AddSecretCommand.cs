using System.CommandLine;
using System.CommandLine.Invocation;
using Comfyg.Cli.Extensions;
using Comfyg.Client;
using Comfyg.Contracts.Requests;
using Comfyg.Contracts.Secrets;

namespace Comfyg.Cli.Commands.Add;

public class AddSecretCommand : Command
{
    private readonly Argument<string> _keyArgument;
    private readonly Argument<string> _valueArgument;

    public AddSecretCommand() : base("secret", "Adds a secret value on the connected Comfyg endpoint")
    {
        _keyArgument = new Argument<string>("key", "The key of the secret value");
        AddArgument(_keyArgument);

        _valueArgument = new Argument<string>("value", "The secret value");
        AddArgument(_valueArgument);

        this.SetHandler(HandleCommandAsync);
    }

    private async Task HandleCommandAsync(InvocationContext context)
    {
        var keyArgument = context.ParseResult.GetValueForArgument(_keyArgument);
        var valueArgument = context.ParseResult.GetValueForArgument(_valueArgument);

        var cancellationToken = context.GetCancellationToken();

        using var client = await State.User.RequireClientAsync(cancellationToken).ConfigureAwait(false);

        await client.Secrets.AddValuesAsync(new AddSecretValuesRequest
        {
            Values = new ISecretValue[]
            {
                new SecretValue(keyArgument, valueArgument)
            }
        }, cancellationToken).ConfigureAwait(false);
    }
}