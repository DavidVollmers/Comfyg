using System.CommandLine;
using Comfyg.Cli;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => { services.AddComfygCommands(); })
    .Build();

var rootCommand = new RootCommand("Comfyg Command-Line Interface");

var commands = host.Services.GetServices<Command>();
foreach (var command in commands)
{
    rootCommand.Add(command);
}

await rootCommand.InvokeAsync(args).ConfigureAwait(false);