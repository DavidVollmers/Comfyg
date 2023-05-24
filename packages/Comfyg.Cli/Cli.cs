using System.Reflection;
using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using Spectre.Console;

namespace Comfyg.Cli;

internal static class Cli
{
    private const string LastUpdateCheck = "LastUpdateCheck";
    
    public static async Task CheckForUpdateAsync(CancellationToken cancellationToken = default)
    {
        var assembly = Assembly.GetAssembly(typeof(Program))!.GetName();
        var version = assembly.Version!;
        var packageId = assembly.Name!;

        var repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
        var resource = await repository.GetResourceAsync<FindPackageByIdResource>(cancellationToken);

        var versions =
            await resource.GetAllVersionsAsync(packageId, new SourceCacheContext(), NullLogger.Instance,
                cancellationToken);

        var latestVersion = versions.Where(v => !v.OriginalVersion.EndsWith("-preview"))
            .OrderByDescending(v => v.Version).First();
        if (version.CompareTo(latestVersion.Version) == -1)
        {
            AnsiConsole.MarkupLine(
                "[bold yellow]You are not using the latest version of the Comfyg Command-Line Interface. This can lead to unexpected behavior. Use the following command to update to the latest version:[/]");
            AnsiConsole.MarkupLine("[bold]dotnet tool update -g Comfyg.Cli[/]");
            AnsiConsole.WriteLine();
        }

        await State.User.StoreAsync(nameof(Comfyg), LastUpdateCheck, DateTime.UtcNow, cancellationToken);
    }
}
