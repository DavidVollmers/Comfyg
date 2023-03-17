using Comfyg.Client;
using Microsoft.Extensions.Configuration;

namespace Comfyg.Configuration;

internal class ComfygConfigurationSource<T> : IConfigurationSource
{
    private readonly Action<ComfygOptions> _optionsConfigurator;

    public ComfygConfigurationSource(Action<ComfygOptions> optionsConfigurator)
    {
        _optionsConfigurator = optionsConfigurator ?? throw new ArgumentNullException(nameof(optionsConfigurator));
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        var options = new ComfygOptions();
        _optionsConfigurator(options);
        if (options.ConnectionString == null)
            throw new InvalidOperationException(
                "Please call ComfygOptions.Connect to specify how to connect to the Comfyg API.");
        var client = options.HttpClient == null
            ? new ComfygClient(options.ConnectionString)
            : new ComfygClient(options.ConnectionString, options.HttpClient);
        return new ComfygConfigurationProvider(client, options.Configuration.ChangeDetectionTimer);
    }
}