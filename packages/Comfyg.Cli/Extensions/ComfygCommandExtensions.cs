using System.CommandLine;
using Comfyg.Cli.Commands;
using Comfyg.Cli.Commands.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Comfyg.Cli.Extensions;

internal static class ComfygCommandExtensions
{
    public static IServiceCollection AddComfygCommands(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<Command, ConnectCommand>();

        serviceCollection.AddScoped<Command, SetupCommand>();
        serviceCollection.AddScoped<SetupClientCommand>();

        return serviceCollection;
    }
}