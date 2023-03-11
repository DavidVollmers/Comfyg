using Comfyg.Authentication.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Comfyg.Authentication;

public static class ComfygAuthenticationExtensions
{
    public static AuthenticationBuilder AddComfygAuthentication(this IServiceCollection serviceCollection)
    {
        if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));

        serviceCollection.AddSingleton<IClientService, ClientService>();

        serviceCollection.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, ComfygJwtBearerOptions>();
        serviceCollection.AddSingleton<ComfygSecurityTokenHandler>();

        return serviceCollection
            .AddAuthentication(options =>
            {
                options.DefaultScheme = nameof(Comfyg);
                options.DefaultForbidScheme = nameof(Comfyg);
            })
            .AddJwtBearer();
    }
}