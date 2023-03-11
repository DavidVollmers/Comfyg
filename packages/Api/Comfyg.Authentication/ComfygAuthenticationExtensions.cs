using Comfyg.Authentication.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Comfyg.Authentication;

public static class ComfygAuthenticationExtensions
{
    public static AuthenticationBuilder AddComfygAuthentication(this IServiceCollection serviceCollection)
    {
        if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));

        serviceCollection.AddScoped<IClientService, ClientService>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var clientService = serviceProvider.GetRequiredService<IClientService>();

        return serviceCollection
            .AddAuthentication(options =>
            {
                options.DefaultScheme = nameof(Comfyg);
                options.DefaultForbidScheme = nameof(Comfyg);
            })
            .AddJwtBearer(nameof(Comfyg), options =>
            {
                options.SecurityTokenValidators.Clear();
                options.SecurityTokenValidators.Add(new ComfygSecurityTokenHandler(clientService));
            });
    }
}