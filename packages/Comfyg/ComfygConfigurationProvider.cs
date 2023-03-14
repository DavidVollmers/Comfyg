using Comfyg.Client;
using Microsoft.Extensions.Configuration;

namespace Comfyg;

internal class ComfygConfigurationProvider : ConfigurationProvider
{
    private readonly ComfygClient _client;

    public ComfygConfigurationProvider(ComfygClient client)
    {
        _client = client;
    }

    public override void Load()
    {
        var result = _client.GetConfigurationAsync().GetAwaiter().GetResult();
        Data = result.ConfigurationValues.ToDictionary(c => c.Key, c => c.Value)!;
    }
}