using Comfyg.Store.Contracts;
using Microsoft.Extensions.Configuration;

namespace Comfyg;

/// <summary>
/// This type provides helper methods to make common use cases for Comfyg easy.
/// </summary>
public static class ComfygExtensions
{
    /// <summary>
    /// Adds key-value data from a Comfyg store to a configuration builder.
    /// </summary>
    /// <param name="configurationBuilder">The configuration builder to add key-values to.</param>
    /// <param name="optionsConfigurator">A callback used to configure Comfyg options.</param>
    /// <returns>The provided configuration builder.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="configurationBuilder"/> or <paramref name="optionsConfigurator"/> is null.</exception>
    public static IConfigurationBuilder AddComfyg(this IConfigurationBuilder configurationBuilder,
        Action<ComfygOptions> optionsConfigurator)
    {
        if (configurationBuilder == null) throw new ArgumentNullException(nameof(configurationBuilder));
        if (optionsConfigurator == null) throw new ArgumentNullException(nameof(optionsConfigurator));

        var options = new ComfygOptions();
        optionsConfigurator(options);

        configurationBuilder.Add(new ComfygSource<IConfigurationValue>(options, options.Configuration));
        configurationBuilder.Add(new ComfygSource<ISettingValue>(options, options.Settings));
        configurationBuilder.Add(new ComfygSource<ISecretValue>(options, options.Secrets));

        return configurationBuilder;
    }
}
