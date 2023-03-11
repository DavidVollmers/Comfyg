using System.CommandLine;
using Comfyg.Cli.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Comfyg.Cli;

public static class ComfygCommandExtensions
{
    public static IServiceCollection AddComfygCommands(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<Command, ConnectCommand>();
        
        return serviceCollection;
    }
}