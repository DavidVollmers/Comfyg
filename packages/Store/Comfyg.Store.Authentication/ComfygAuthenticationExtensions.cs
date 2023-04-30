using Azure.Data.Tables;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Core;
using Comfyg.Store.Core.Abstractions;
using Comfyg.Store.Core.Abstractions.Secrets;
using Comfyg.Store.Core.Secrets;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Comfyg.Store.Authentication;

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

        TableServiceClient TableServiceClientProvider()
        {
            var options = OptionsProvider();
            if (options.AzureTableStorageConnectionString == null)
                throw new InvalidOperationException("Missing AzureTableStorageConnectionString option.");
            return new TableServiceClient(options.AzureTableStorageConnectionString);
        }

        IBlobService BlobServiceProvider()
        {
            var options = OptionsProvider();
            if (options.AzureBlobStorageConnectionString == null)
                throw new InvalidOperationException("Missing AzureBlobStorageConnectionString option.");
            return new BlobService(nameof(Comfyg) + nameof(Authentication),
                new BlobServiceClient(options.AzureBlobStorageConnectionString));
        }

        ISecretService SecretServiceProvider(IServiceProvider provider)
        {
            var options = OptionsProvider();

            if (options.EncryptionKey != null)
            {
                return new EncryptionBasedSecretService(options.EncryptionKey);
            }

            if (!options.UseKeyVault)
                throw new InvalidOperationException(
                    "Neither encryption nor Azure Key Vault is configured. Use either AuthenticationOptions.UseEncryption or AuthenticationOptions.UseKeyVault to configure secret handling.");

            return new KeyVaultSecretService(nameof(Comfyg) + nameof(Authentication),
                provider.GetRequiredService<SecretClient>());
        }

        serviceCollection.AddSingleton<IClientService, ClientService>(provider =>
            new ClientService(TableServiceClientProvider(), SecretServiceProvider(provider), BlobServiceProvider()));

        serviceCollection.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, ComfygJwtBearerOptions>();
        serviceCollection.AddSingleton<ComfygSecurityTokenHandler>();

        return serviceCollection
            .AddAuthentication(nameof(Comfyg))
            .AddJwtBearer(nameof(Comfyg), options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false, ValidateIssuer = false
                };
            });
    }
}
