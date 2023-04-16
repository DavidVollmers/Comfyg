using System.CommandLine;
using System.CommandLine.Invocation;
using Comfyg.Cli.Extensions;
using Comfyg.Store.Contracts;
using Spectre.Console;

namespace Comfyg.Cli.Commands.Set;

internal class SetPermissionsCommand : Command
{
    private readonly Argument<string> _clientIdArgument;
    private readonly Option<Permissions[]> _permissionsOption;

    public SetPermissionsCommand(SetConfigurationPermissionsCommand setConfigurationPermissionsCommand,
        SetSecretPermissionsCommand setSecretPermissionsCommand,
        SetSettingPermissionsCommand setSettingPermissionsCommand)
        : base("permissions", "Sets permissions on all permitted key-value pairs for a different client.")
    {
        if (setConfigurationPermissionsCommand == null)
            throw new ArgumentNullException(nameof(setConfigurationPermissionsCommand));
        if (setSecretPermissionsCommand == null) throw new ArgumentNullException(nameof(setSecretPermissionsCommand));
        if (setSettingPermissionsCommand == null) throw new ArgumentNullException(nameof(setSettingPermissionsCommand));

        AddCommand(setConfigurationPermissionsCommand);
        AddCommand(setSecretPermissionsCommand);
        AddCommand(setSettingPermissionsCommand);

        _clientIdArgument = new Argument<string>("CLIENT_ID", "The ID of the client to set the permissions for.");
        AddArgument(_clientIdArgument);

        _permissionsOption = new Option<Permissions[]>(new []
        {
            "-p",
            "--permissions"
        }, "The kind of permissions which will be set for the client. Defaults to `read`.")
        {
            Arity = ArgumentArity.ZeroOrMore,
            AllowMultipleArgumentsPerToken = true
        };
        AddOption(_permissionsOption);

        this.SetHandler(HandleCommandAsync);
    }

    private async Task HandleCommandAsync(InvocationContext context)
    {
        var clientIdArgument = context.ParseResult.GetValueForArgument(_clientIdArgument);
        var permissionsOption = context.ParseResult.GetValueForOption(_permissionsOption)!;

        var cancellationToken = context.GetCancellationToken();

        using var client = await State.User.RequireClientAsync(cancellationToken);

        await client.SetPermissionsAsync(clientIdArgument, permissionsOption.ToFlags(), cancellationToken);

        AnsiConsole.MarkupLine($"[bold green]Successfully set permissions for \"{clientIdArgument}\"[/]");
    }
}
