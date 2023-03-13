using System.CommandLine;
using Comfyg.Cli;
using Comfyg.Cli.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => { services.AddComfygCommands(); })
    .Build();

var rootCommand = new RootCommand("Comfyg Command-Line Interface")
{
    Name = "comfyg"
};

var commands = host.Services.GetServices<Command>();
foreach (var command in commands)
{
    rootCommand.Add(command);
}

if (args.Length == 0)
{
    AnsiConsole.Write(new FigletText(nameof(Comfyg)).Color(CliConstants.PrimaryColor));
}

await rootCommand.InvokeAsync(args).ConfigureAwait(false);