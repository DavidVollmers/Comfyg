using System.CommandLine;
using Comfyg.Cli.Commands;
using Comfyg.Cli.Commands.Add;
using Comfyg.Cli.Commands.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Comfyg.Cli.Extensions;

internal static class ComfygCommandExtensions
{
    public static IServiceCollection AddComfygCommands(this IServiceCollection serviceCollection)
    {
        if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
        
        serviceCollection.AddScoped<Command, ConnectCommand>();

        serviceCollection.AddScoped<SetupClientCommand>();
        serviceCollection.AddScoped<Command, SetupCommand>();

        serviceCollection.AddScoped<AddConfigurationCommand>();
        serviceCollection.AddScoped<Command, AddCommand>();

        return serviceCollection;
    }
}