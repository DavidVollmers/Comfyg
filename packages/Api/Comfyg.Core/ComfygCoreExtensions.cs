using Azure.Data.Tables;
using Azure.Security.KeyVault.Secrets;
using Comfyg.Contracts.Configuration;
using Comfyg.Contracts.Secrets;
using Comfyg.Contracts.Settings;
using Comfyg.Core.Abstractions;
using Comfyg.Core.Abstractions.Changes;
using Comfyg.Core.Abstractions.Permissions;
using Comfyg.Core.Abstractions.Secrets;
using Comfyg.Core.Changes;
using Comfyg.Core.Configuration;
using Comfyg.Core.Permissions;
using Comfyg.Core.Secrets;
using Comfyg.Core.Settings;
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

        TableServiceClient TableServiceClientProvider()
        {
            var options = OptionsProvider();
            if (options.AzureTableStorageConnectionString == null)
                throw new InvalidOperationException("Missing AzureTableStorageConnectionString option.");
            return new TableServiceClient(options.AzureTableStorageConnectionString);
        }

        serviceCollection.AddScoped<IChangeService, ChangeService>(provider => new ChangeService(
            //TODO make systemId configurable
            nameof(Comfyg),
            TableServiceClientProvider(),
            provider.GetRequiredService<IPermissionService>()));

        serviceCollection.AddScoped<IPermissionService, PermissionService>(_ =>
            new PermissionService(
                //TODO make systemId configurable
                nameof(Comfyg),
                TableServiceClientProvider()));

        serviceCollection
            .AddScoped<IValueService<IConfigurationValue>, ValueService<IConfigurationValue, ConfigurationValueEntity>>(
                provider => new ValueService<IConfigurationValue, ConfigurationValueEntity>(
                    //TODO make systemId configurable
                    nameof(Comfyg),
                    TableServiceClientProvider(),
                    provider.GetRequiredService<IPermissionService>(),
                    provider.GetRequiredService<IChangeService>()));

        serviceCollection.AddScoped<IValueService<ISettingValue>, ValueService<ISettingValue, SettingValueEntity>>(
            provider => new ValueService<ISettingValue, SettingValueEntity>(
                //TODO make systemId configurable
                nameof(Comfyg),
                TableServiceClientProvider(),
                provider.GetRequiredService<IPermissionService>(),
                provider.GetRequiredService<IChangeService>()));

        serviceCollection.AddScoped<IValueService<ISecretValue>, ValueService<ISecretValue, SecretValueEntity>>(
            provider => new ValueService<ISecretValue, SecretValueEntity>(
                //TODO make systemId configurable
                nameof(Comfyg),
                TableServiceClientProvider(),
                provider.GetRequiredService<IPermissionService>(),
                provider.GetRequiredService<IChangeService>()));

        serviceCollection.AddScoped<ISecretService>(provider =>
        {
            var options = OptionsProvider();

            if (options.EncryptionKey != null)
            {
                return new EncryptionBasedSecretService(options.EncryptionKey);
            }

            if (!options.UseKeyVault)
                throw new InvalidOperationException(
                    "Neither encryption nor Azure Key Vault is configured. Use either ComfygOptions.UseEncryption or ComfygOptions.UseKeyVault to configure secret handling.");

            //TODO make systemId configurable
            return new KeyVaultSecretService(nameof(Comfyg), provider.GetRequiredService<SecretClient>());
        });

        return serviceCollection;
    }
}
