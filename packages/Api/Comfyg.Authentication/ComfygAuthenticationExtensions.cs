using Comfyg.Authentication.Abstractions;
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
                throw new InvalidOperationException("Missing AzureTableStorageConnectionString");
            return new StorageContext(options.AzureTableStorageConnectionString);
        }

        ISecretService SecretServiceProvider()
        {
            var options = OptionsProvider();
            if (options.EncryptionKey == null)
                throw new InvalidOperationException("Missing EncryptionKey");
            return new EncryptionBasedSecretService(options.EncryptionKey);
        }

        serviceCollection.AddSingleton<IClientService, ClientService>(_ =>
        {
            var secretService = SecretServiceProvider();
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