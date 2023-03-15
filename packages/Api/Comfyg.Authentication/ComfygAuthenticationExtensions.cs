using Azure.Security.KeyVault.Secrets;
using Comfyg.Authentication.Abstractions;
using Comfyg.Core.Abstractions.Changes;
using Comfyg.Core.Abstractions.Permissions;
using Comfyg.Core.Abstractions.Secrets;
using Comfyg.Core.Secrets;
using CoreHelpers.WindowsAzure.Storage.Table;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Comfyg.Authentication;

public static class ComfygAuthenticationExtensions
{
    public static AuthenticationBuilder AddComfygAuthentication(this IServiceCollection serviceCollection,
        Action<ComfygAuthenticationOptions> optionsConfigurator)
    {
        if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
        if (optionsConfigurator == null) throw new ArgumentNullException(nameof(optionsConfigurator));

        ComfygAuthenticationOptions OptionsProvider()
        {
            var options = new ComfygAuthenticationOptions();
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

        ISecretService SecretServiceProvider(IServiceProvider provider)
        {
            var options = OptionsProvider();

            if (options.EncryptionKey != null)
            {
                //TODO make systemId configurable
                return new EncryptionBasedSecretService(nameof(Authentication), options.EncryptionKey);
            }

            if (!options.UseKeyVault)
                throw new InvalidOperationException(
                    "Neither encryption nor Azure Key Vault is configured. Use either ComfygAuthenticationOptions.UseEncryption or ComfygAuthenticationOptions.UseKeyVault to configure secret handling.");

            var storageContext = StorageContextProvider();
            //TODO make systemId configurable
            return new KeyVaultSecretService(nameof(Authentication), provider.GetRequiredService<SecretClient>(),
                storageContext, provider.GetRequiredService<IChangeService>(),
                provider.GetRequiredService<IPermissionService>());
        }

        serviceCollection.AddSingleton<IClientService, ClientService>(provider =>
        {
            var secretService = SecretServiceProvider(provider);
            var storageContext = StorageContextProvider();
            return new ClientService(storageContext, secretService);
        });

        serviceCollection.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, ComfygJwtBearerOptions>();
        serviceCollection.AddSingleton<ComfygSecurityTokenHandler>();

        return serviceCollection
            .AddAuthentication(nameof(Comfyg))
            .AddJwtBearer(nameof(Comfyg), options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false
                };
            });
    }
}