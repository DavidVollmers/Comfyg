using Comfyg.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Comfyg;

internal class ComfygConfigurationProvider : ConfigurationProvider
{
    private readonly ComfygClient _client;

    public ComfygConfigurationProvider(ComfygClient client, TimeSpan? changeDetectionInterval = null)
    {
        _client = client;

        if (changeDetectionInterval.HasValue)
        {
            var changeDetector = new ChangeDetector(client, changeDetectionInterval.Value);
            //TODO only load changes
            ChangeToken.OnChange(changeDetector.GetChangeToken, Load);
        }
    }

    public override void Load()
    {
        var result = _client.GetConfigurationAsync().GetAwaiter().GetResult();
        Data = result.ConfigurationValues.ToDictionary(c => c.Key, c => c.Value)!;
    }
}