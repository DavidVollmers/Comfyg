using Comfyg.Core.Abstractions.Changes;
using Comfyg.Core.Abstractions.Configuration;
using Comfyg.Core.Abstractions.Permissions;
using Comfyg.Core.Changes;
using Comfyg.Core.Configuration;
using Comfyg.Core.Permissions;
using CoreHelpers.WindowsAzure.Storage.Table;
using Microsoft.Extensions.DependencyInjection;

namespace Comfyg.Core;

public static class ComfygCoreExtensions
{
    public static IServiceCollection AddComfyg(this IServiceCollection serviceCollection,
        Action<ComfygOptions> optionsConfigurator)
    {
        if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
        if (optionsConfigurator == null) throw new ArgumentNullException(nameof(optionsConfigurator));

        ComfygOptions OptionsProvider()
        {
            var options = new ComfygOptions();
            optionsConfigurator(options);
            return options;
        }

        IStorageContext StorageContextProvider()
        {
            var options = OptionsProvider();
            if (options.AzureTableStorageConnectionString == null)
                throw new InvalidOperationException("Missing AzureTableStorageConnectionString option.");
            return new StorageContext(options.AzureTableStorageConnectionString);
        }

        serviceCollection.AddScoped<IChangeService, ChangeService>(provider =>
        {
            var storageContext = StorageContextProvider();
            //TODO make systemId configurable
            return new ChangeService(nameof(Comfyg), storageContext, provider.GetRequiredService<IPermissionService>());
        });

        serviceCollection.AddScoped<IPermissionService, PermissionService>(_ =>
        {
            var storageContext = StorageContextProvider();
            //TODO make systemId configurable
            return new PermissionService(nameof(Comfyg), storageContext);
        });

        serviceCollection.AddScoped<IConfigurationService, ConfigurationService>(provider =>
        {
            var storageContext = StorageContextProvider();
            //TODO make systemId configurable
            return new ConfigurationService(nameof(Comfyg), storageContext,
                provider.GetRequiredService<IPermissionService>(),
                provider.GetRequiredService<IChangeService>());
        });

        return serviceCollection;
    }
}