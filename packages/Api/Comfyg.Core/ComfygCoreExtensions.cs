using Comfyg.Core.Abstractions.Configuration;
using Comfyg.Core.Configuration;
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
                throw new InvalidOperationException("Missing AzureTableStorageConnectionString");
            return new StorageContext(options.AzureTableStorageConnectionString);
        }

        serviceCollection.AddScoped<IConfigurationService, ConfigurationService>(_ =>
        {
            var storageContext = StorageContextProvider();
            //TODO make systemId configurable
            return new ConfigurationService(nameof(Comfyg), storageContext);
        });

        return serviceCollection;
    }
}