using System.CommandLine;
using Comfyg.Cli.Commands;
using Comfyg.Cli.Commands.Add;
using Comfyg.Cli.Commands.Export;
using Comfyg.Cli.Commands.Import;
using Comfyg.Cli.Commands.Set;
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
        serviceCollection.AddScoped<SetupLocalhostCommand>();
        serviceCollection.AddScoped<Command, SetupCommand>();

        serviceCollection.AddScoped<AddConfigurationCommand>();
        serviceCollection.AddScoped<AddSettingCommand>();
        serviceCollection.AddScoped<AddSecretCommand>();
        serviceCollection.AddScoped<Command, AddCommand>();

        serviceCollection.AddScoped<ImportConfigurationCommand>();
        serviceCollection.AddScoped<ImportSettingsCommand>();
        serviceCollection.AddScoped<ImportSecretsCommand>();
        serviceCollection.AddScoped<Command, ImportCommand>();

        serviceCollection.AddScoped<ExportConfigurationCommand>();
        serviceCollection.AddScoped<ExportSettingsCommand>();
        serviceCollection.AddScoped<ExportSecretsCommand>();
        serviceCollection.AddScoped<Command, ExportCommand>();

        serviceCollection.AddScoped<SetConfigurationPermissionsCommand>();
        serviceCollection.AddScoped<SetSecretPermissionsCommand>();
        serviceCollection.AddScoped<SetSettingPermissionsCommand>();
        serviceCollection.AddScoped<SetPermissionsCommand>();
        serviceCollection.AddScoped<Command, SetCommand>();

        return serviceCollection;
    }
}
