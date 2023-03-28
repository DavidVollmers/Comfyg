using Comfyg.Store.Contracts;
using Spectre.Console;

namespace Comfyg.Cli.Extensions;

internal static class ComfygSpectreExtensions
{
    public static Table ToTable(this IClient client, string? clientSecret = null)
    {
        if (client == null) throw new ArgumentNullException(nameof(client));

        var table = new Table();

        table.AddColumn(nameof(IClient.ClientId));
        table.AddColumn(nameof(IClient.FriendlyName));
        if (clientSecret != null) table.AddColumn(nameof(IClient.ClientSecret));

        if (clientSecret != null) table.AddRow(client.ClientId, client.FriendlyName, $"[bold]{clientSecret}[/]");
        else table.AddRow(client.ClientId, client.FriendlyName);

        return table;
    }
}
