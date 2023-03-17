using Comfyg.Client;
using Comfyg.Contracts;
using Comfyg.Timing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Comfyg.Configuration;

internal class ComfygConfigurationProvider : ConfigurationProvider, IDisposable
{
    private readonly ComfygClient _client;
    private readonly ChangeDetector? _changeDetector;

    public ComfygConfigurationProvider(ComfygClient client, ITimer? timer = null)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));

        if (timer == null) return;
        _changeDetector = new ChangeDetector(client, timer);
        ChangeToken.OnChange(_changeDetector.GetChangeToken, () => { LoadDiff(_changeDetector.LastDetectionAt); });
    }

    public override void Load()
    {
        var result = _client.GetConfigurationValuesAsync().GetAwaiter().GetResult();
        SetData(result.Values);
    }

    private void LoadDiff(DateTime since)
    {
        var result = _client.GetConfigurationValuesFromDiffAsync(since).GetAwaiter().GetResult();
        SetData(result.Values, false);
    }

    private void SetData(IEnumerable<IComfygValue> values, bool reset = true)
    {
        if (reset)
        {
            Data = values.ToDictionary(c => c.Key, c => c.Value)!;
        }
        else
        {
            foreach (var value in values)
            {
                Set(value.Key, value.Value);
            }
        }
    }

    public void Dispose()
    {
        _changeDetector?.Dispose();
        _client.Dispose();
    }
}