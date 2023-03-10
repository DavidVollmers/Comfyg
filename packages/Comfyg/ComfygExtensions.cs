using Microsoft.Extensions.Configuration;

namespace Comfyg;

public static class ComfygExtensions
{
    public static IConfigurationBuilder AddComfyg(this IConfigurationBuilder configurationBuilder,
        Action<ComfygOptions> optionsProvider)
    {
        if (configurationBuilder == null) throw new ArgumentNullException(nameof(configurationBuilder));
        if (optionsProvider == null) throw new ArgumentNullException(nameof(optionsProvider));

        configurationBuilder.Add(new ComfygSource(optionsProvider));

        return configurationBuilder;
    }
}