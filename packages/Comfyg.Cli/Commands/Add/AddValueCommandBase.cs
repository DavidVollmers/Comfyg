using System.CommandLine;
using System.CommandLine.Invocation;
using Comfyg.Cli.Extensions;
using Comfyg.Store.Contracts;
using Spectre.Console;

namespace Comfyg.Cli.Commands.Add;

internal abstract class AddValueCommandBase<T> : Command where T : IComfygValue
{
    private readonly Argument<string> _keyArgument;
    private readonly Argument<string> _valueArgument;

    protected AddValueCommandBase(string name, string? description = null) : base(name, description)
    {
        _keyArgument = new Argument<string>("KEY", "The key of the key-value pair.");
        AddArgument(_keyArgument);

        _valueArgument = new Argument<string>("VALUE", "The value of the key-value pair.");
        AddArgument(_valueArgument);

        this.SetHandler(HandleCommandAsync);
    }

    protected abstract T BuildValue(string key, string value);

    private async Task HandleCommandAsync(InvocationContext context)
    {
        var keyArgument = context.ParseResult.GetValueForArgument(_keyArgument);
        var valueArgument = context.ParseResult.GetValueForArgument(_valueArgument);

        var cancellationToken = context.GetCancellationToken();

        using var client = await State.User.RequireClientAsync(cancellationToken);

        await client.AddValuesAsync(new[] { BuildValue(keyArgument, valueArgument) }, cancellationToken);

        AnsiConsole.MarkupLine($"[bold green]Successfully added value for \"{keyArgument}\"[/]");
    }
}
