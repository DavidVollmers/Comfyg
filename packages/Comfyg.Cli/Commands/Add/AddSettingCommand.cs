using System.CommandLine;
using System.CommandLine.Invocation;
using Comfyg.Cli.Extensions;
using Comfyg.Client;
using Comfyg.Contracts.Requests;
using Comfyg.Contracts.Settings;

namespace Comfyg.Cli.Commands.Add;

public class AddSettingCommand : Command
{
    private readonly Argument<string> _keyArgument;
    private readonly Argument<string> _valueArgument;

    public AddSettingCommand() : base("setting", "Adds a setting value on the connected Comfyg endpoint")
    {
        _keyArgument = new Argument<string>("key", "The key of the setting value");
        AddArgument(_keyArgument);

        _valueArgument = new Argument<string>("value", "The setting value");
        AddArgument(_valueArgument);

        this.SetHandler(HandleCommandAsync);
    }

    private async Task HandleCommandAsync(InvocationContext context)
    {
        var keyArgument = context.ParseResult.GetValueForArgument(_keyArgument);
        var valueArgument = context.ParseResult.GetValueForArgument(_valueArgument);

        var cancellationToken = context.GetCancellationToken();

        using var client = await State.User.RequireClientAsync(cancellationToken).ConfigureAwait(false);

        await client.Settings.AddValuesAsync(new AddSettingValuesRequest
        {
            Values = new ISettingValue[]
            {
                new SettingValue(keyArgument, valueArgument)
            }
        }, cancellationToken).ConfigureAwait(false);
    }
}