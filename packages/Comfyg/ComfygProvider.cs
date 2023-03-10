using Comfyg.Client;
using Microsoft.Extensions.Configuration;

namespace Comfyg;

internal class ComfygProvider : ConfigurationProvider
{
    private readonly ComfygClient _client;

    public ComfygProvider(ComfygClient client)
    {
        _client = client;
    }

    public override void Load()
    {
        
    }
}