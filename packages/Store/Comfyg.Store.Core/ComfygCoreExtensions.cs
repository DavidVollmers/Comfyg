using Azure.Data.Tables;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Comfyg.Store.Contracts;
using Comfyg.Store.Core.Abstractions;
using Comfyg.Store.Core.Abstractions.Changes;
using Comfyg.Store.Core.Abstractions.Permissions;
using Comfyg.Store.Core.Abstractions.Secrets;
using Comfyg.Store.Core.Changes;
using Comfyg.Store.Core.Configuration;
using Comfyg.Store.Core.Permissions;
using Comfyg.Store.Core.Secrets;
using Comfyg.Store.Core.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Comfyg.Store.Core;

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

            if (options.KeyVaultUri == null)
                throw new InvalidOperationException(
                    "Neither encryption nor Azure Key Vault is configured. Use either ComfygOptions.UseEncryption or ComfygOptions.UseKeyVault to configure secret handling.");

            var secretClient = new SecretClient(options.KeyVaultUri, new DefaultAzureCredential());

            //TODO make systemId configurable
            return new KeyVaultSecretService(nameof(Comfyg), secretClient);
        });

        return serviceCollection;
    }
}
