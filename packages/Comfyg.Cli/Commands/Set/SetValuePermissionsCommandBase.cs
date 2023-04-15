using System.CommandLine;
using System.CommandLine.Invocation;
using Comfyg.Cli.Extensions;
using Comfyg.Store.Contracts;
using Spectre.Console;

namespace Comfyg.Cli.Commands.Set;

internal abstract class SetValuePermissionsCommandBase<T> : Command where T : IComfygValue
{
    private readonly Argument<string> _keyArgument;
    private readonly Argument<string> _clientIdArgument;
    private readonly Option<Permissions[]> _permissionsOption;

    protected SetValuePermissionsCommandBase(string name, string? description = null) : base(name, description)
    {
        _keyArgument = new Argument<string>("KEY", "The key of the key-value pair to set the permission for.");
        AddArgument(_keyArgument);

        _clientIdArgument = new Argument<string>("CLIENT_ID", "The ID of the client to set the permission for.");
        AddArgument(_clientIdArgument);

        _permissionsOption =
            new Option<Permissions[]>(new[] { "-p", "--permissions" },
                "The kind of permissions which will be set for the client. Defaults to `read`.")
            {
                Arity = ArgumentArity.ZeroOrMore, AllowMultipleArgumentsPerToken = true
            };
        _permissionsOption.SetDefaultValue(Permissions.Read);
        AddOption(_permissionsOption);

        this.SetHandler(HandleCommandAsync);
    }

    private async Task HandleCommandAsync(InvocationContext context)
    {
        var keyArgument = context.ParseResult.GetValueForArgument(_keyArgument);
        var clientIdArgument = context.ParseResult.GetValueForArgument(_clientIdArgument);
        var permissionsOption = context.ParseResult.GetValueForOption(_permissionsOption)!;

        var cancellationToken = context.GetCancellationToken();

        using var client = await State.User.RequireClientAsync(cancellationToken);

        await client.SetPermissionAsync<T>(clientIdArgument, keyArgument, permissionsOption.ToFlags(),
            cancellationToken);

        AnsiConsole.MarkupLine($"[bold green]Successfully set permission for \"{clientIdArgument}\"[/]");
    }
}
