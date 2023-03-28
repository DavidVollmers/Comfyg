﻿using System.CommandLine;
using System.CommandLine.Invocation;
using Comfyg.Cli.Extensions;
using Comfyg.Client;
using Comfyg.Store.Contracts;
using Spectre.Console;

namespace Comfyg.Cli.Commands.Add;

internal class AddConfigurationCommand : Command
{
    private readonly Argument<string> _keyArgument;
    private readonly Argument<string> _valueArgument;

    public AddConfigurationCommand() : base("config",
        "Adds a key-value pair as a configuration value to the connected Comfyg store.")
    {
        _keyArgument = new Argument<string>("KEY", "The key of the key-value pair.");
        AddArgument(_keyArgument);

        _valueArgument = new Argument<string>("VALUE", "The value of the key-value pair.");
        AddArgument(_valueArgument);

        this.SetHandler(HandleCommandAsync);
    }

    private async Task HandleCommandAsync(InvocationContext context)
    {
        var keyArgument = context.ParseResult.GetValueForArgument(_keyArgument);
        var valueArgument = context.ParseResult.GetValueForArgument(_valueArgument);

        var cancellationToken = context.GetCancellationToken();

        using var client = await State.User.RequireClientAsync(cancellationToken);

        await client.Configuration.AddValuesAsync(
            new IConfigurationValue[] { new ConfigurationValue(keyArgument, valueArgument) }, cancellationToken);

        AnsiConsole.MarkupLine($"[bold green]Successfully added the configuration value for \"{keyArgument}\"[/]");
    }
}
