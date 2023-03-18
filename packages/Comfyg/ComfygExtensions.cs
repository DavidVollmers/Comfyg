using Comfyg.Contracts.Configuration;
using Comfyg.Contracts.Secrets;
using Comfyg.Contracts.Settings;
using Microsoft.Extensions.Configuration;

namespace Comfyg;

public static class ComfygExtensions
{
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