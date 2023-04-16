using System.CommandLine;
using System.CommandLine.Invocation;
using Comfyg.Cli.Extensions;
using Comfyg.Store.Contracts;
using Spectre.Console;

namespace Comfyg.Cli.Commands.Tag;

internal abstract class TagValueCommandBase<T> : Command where T : IComfygValue
{
    private readonly Argument<string> _keyArgument;
    private readonly Argument<string> _tagArgument;
    private readonly Option<string> _versionOption;

    protected TagValueCommandBase(string name, string? description = null) : base(name, description)
    {
        _keyArgument = new Argument<string>("KEY", "The key of the key-value pair.");
        AddArgument(_keyArgument);

        _tagArgument = new Argument<string>("TAG", "The identifier of the tag.");
        AddArgument(_tagArgument);

        _versionOption = new Option<string>(new[] { "-v", "--version" },
            "The version of the key-value pair. Defaults to `latest`.");
        _versionOption.SetDefaultValue(ComfygConstants.LatestVersion);
        AddOption(_versionOption);

        this.SetHandler(HandleCommandAsync);
    }

    private async Task HandleCommandAsync(InvocationContext context)
    {
        var keyArgument = context.ParseResult.GetValueForArgument(_keyArgument);
        var tagArgument = context.ParseResult.GetValueForArgument(_tagArgument);
        var versionOption = context.ParseResult.GetValueForOption(_versionOption)!;

        var cancellationToken = context.GetCancellationToken();

        using var client = await State.User.RequireClientAsync(cancellationToken);

        var result = await client.TagValueAsync<T>(keyArgument, tagArgument, versionOption, cancellationToken);

        AnsiConsole.MarkupLine($"[bold green]Successfully tagged value. (Key: \"{result.Key}\", Version: {result.Version})[/]");
    }
}
