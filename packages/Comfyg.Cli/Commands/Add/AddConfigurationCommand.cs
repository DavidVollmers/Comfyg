﻿using System.CommandLine;
using System.CommandLine.Invocation;
using Comfyg.Cli.Extensions;
using Comfyg.Contracts.Configuration;
using Comfyg.Contracts.Requests;

namespace Comfyg.Cli.Commands.Add;

//TODO tags
public class AddConfigurationCommand : Command
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

        using var client = await State.User.RequireClientAsync(cancellationToken).ConfigureAwait(false);

        await client.AddConfigurationAsync(new AddConfigurationRequest
        {
            ConfigurationValues = new IConfigurationValue[]
            {
                new ConfigurationValue
                {
                    Key = keyArgument,
                    Value = valueArgument
                }
            }
        }, cancellationToken).ConfigureAwait(false);
    }
}