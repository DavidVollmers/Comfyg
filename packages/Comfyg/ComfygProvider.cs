using Comfyg.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Comfyg;

internal class ComfygProvider : IConfigurationProvider
{
    private readonly ComfygClient _client;

    public ComfygProvider(ComfygClient client)
    {
        _client = client;
    }
    
    public bool TryGet(string key, out string? value)
    {
        throw new NotImplementedException();
    }

    public void Set(string key, string? value)
    {
        throw new NotImplementedException();
    }

    public IChangeToken GetReloadToken()
    {
        throw new NotImplementedException();
    }

    public void Load()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string? parentPath)
    {
        throw new NotImplementedException();
    }
}