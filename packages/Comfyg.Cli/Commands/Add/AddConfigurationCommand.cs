using System.CommandLine;
using System.CommandLine.Invocation;
using Comfyg.Cli.Extensions;
using Comfyg.Client;
using Comfyg.Contracts.Configuration;
using Comfyg.Contracts.Requests;
using Spectre.Console;

namespace Comfyg.Cli.Commands.Add;

internal class AddConfigurationCommand : Command
{
    private readonly Argument<string> _keyArgument;
    private readonly Argument<string> _valueArgument;

    public AddConfigurationCommand() : base("config", "Adds a configuration value on the connected Comfyg endpoint")
    {
        _keyArgument = new Argument<string>("key", "The key of the configuration value");
        AddArgument(_keyArgument);

        _valueArgument = new Argument<string>("value", "The configuration value");
        AddArgument(_valueArgument);

        this.SetHandler(HandleCommandAsync);
    }

    private async Task HandleCommandAsync(InvocationContext context)
    {
        var keyArgument = context.ParseResult.GetValueForArgument(_keyArgument);
        var valueArgument = context.ParseResult.GetValueForArgument(_valueArgument);

        var cancellationToken = context.GetCancellationToken();

        using var client = await State.User.RequireClientAsync(cancellationToken);

        await client.Configuration.AddValuesAsync(new AddConfigurationValuesRequest
        {
            Values = new IConfigurationValue[]
            {
                new ConfigurationValue(keyArgument, valueArgument)
            }
        }, cancellationToken);

        AnsiConsole.MarkupLine($"[bold green]Successfully added the configuration value for \"{keyArgument}\"[/]");
    }
}
