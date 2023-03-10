using Comfyg.Client;
using Microsoft.Extensions.Configuration;

namespace Comfyg;

internal class ComfygSource : IConfigurationSource
{
    private readonly Action<ComfygOptions> _optionsProvider;

    public ComfygSource(Action<ComfygOptions> optionsProvider)
    {
        _optionsProvider = optionsProvider;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        var options = new ComfygOptions();
        _optionsProvider(options);
        if (options.ConnectionString == null)
            throw new InvalidOperationException(
                "Please call ComfygOptions.Connect to specify how to connect to Comfyg.");
        var client = new ComfygClient(options.ConnectionString);
        return new ComfygProvider(client);
    }
}