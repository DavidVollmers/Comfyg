using Comfyg.Client;
using Comfyg.Timing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Comfyg;

internal class ComfygConfigurationProvider : ConfigurationProvider, IDisposable
{
    private readonly ComfygClient _client;
    private readonly ChangeDetector? _changeDetector;

    public ComfygConfigurationProvider(ComfygClient client, ITimer? timer = null)
    {
        _client = client;

        if (timer != null)
        {
            _changeDetector = new ChangeDetector(client, timer);
            //TODO only load changes
            ChangeToken.OnChange(_changeDetector.GetChangeToken, Load);
        }
    }

    public override void Load()
    {
        var result = _client.GetConfigurationAsync().GetAwaiter().GetResult();
        Data = result.ConfigurationValues.ToDictionary(c => c.Key, c => c.Value)!;
    }

    public void Dispose()
    {
        _changeDetector?.Dispose();
        _client.Dispose();
    }
}