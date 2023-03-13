using Comfyg.Client;
using Microsoft.Extensions.Configuration;

namespace Comfyg;

internal class ComfygSource : IConfigurationSource
{
    private readonly Action<ComfygOptions> _optionsConfigurator;

    public ComfygSource(Action<ComfygOptions> optionsConfigurator)
    {
        _optionsConfigurator = optionsConfigurator;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        var options = new ComfygOptions();
        _optionsConfigurator(options);
        if (options.ConnectionString == null)
            throw new InvalidOperationException(
                "Please call ComfygOptions.Connect to specify how to connect to Comfyg.");
        var client = new ComfygClient(options.ConnectionString);
        return new ComfygProvider(client);
    }
}