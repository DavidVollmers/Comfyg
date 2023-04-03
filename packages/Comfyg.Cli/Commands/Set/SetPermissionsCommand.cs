using System.CommandLine;
using System.CommandLine.Invocation;
using Comfyg.Cli.Extensions;
using Spectre.Console;

namespace Comfyg.Cli.Commands.Set;

internal class SetPermissionsCommand : Command
{
    private readonly Argument<string> _clientIdArgument;
    
    public SetPermissionsCommand(SetConfigurationPermissionsCommand setConfigurationPermissionsCommand,
        SetSecretPermissionsCommand setSecretPermissionsCommand,
        SetSettingPermissionsCommand setSettingPermissionsCommand)
        : base("permissions", "Sets the permissions of the connected client for a different client.")
    {
        if (setConfigurationPermissionsCommand == null) throw new ArgumentNullException(nameof(setConfigurationPermissionsCommand));
        if (setSecretPermissionsCommand == null) throw new ArgumentNullException(nameof(setSecretPermissionsCommand));
        if (setSettingPermissionsCommand == null) throw new ArgumentNullException(nameof(setSettingPermissionsCommand));
        
        AddCommand(setConfigurationPermissionsCommand);
        AddCommand(setSecretPermissionsCommand);
        AddCommand(setSettingPermissionsCommand);
        
        _clientIdArgument = new Argument<string>("CLIENT_ID", "The ID of the client to set the permissions for.");
        AddArgument(_clientIdArgument);

        this.SetHandler(HandleCommandAsync);
    }

    private async Task HandleCommandAsync(InvocationContext context)
    {
        var clientIdArgument = context.ParseResult.GetValueForArgument(_clientIdArgument);
        
        var cancellationToken = context.GetCancellationToken();

        using var client = await State.User.RequireClientAsync(cancellationToken);

        await client.SetPermissionsAsync(clientIdArgument, cancellationToken);

        AnsiConsole.MarkupLine($"[bold green]Successfully set permissions for \"{clientIdArgument}\"[/]");
    }
}
