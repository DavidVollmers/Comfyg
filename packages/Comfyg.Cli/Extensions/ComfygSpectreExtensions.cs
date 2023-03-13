using Comfyg.Contracts.Authentication;
using Spectre.Console;

namespace Comfyg.Cli.Extensions;

internal static class ComfygSpectreExtensions
{
    public static Table ToTable(this IClient client, string? clientSecret = null)
    {
        var table = new Table();

        table.AddColumn(nameof(IClient.ClientId));
        table.AddColumn(nameof(IClient.FriendlyName));
        if (clientSecret != null) table.AddColumn(nameof(IClient.ClientSecret));

        table.AddRow(nameof(IClient.ClientId), client.ClientId);
        table.AddRow(nameof(IClient.FriendlyName), client.FriendlyName);
        if (clientSecret != null) table.AddRow(nameof(IClient.ClientSecret), $"[bold]{clientSecret}[/]");

        return table;
    }
}