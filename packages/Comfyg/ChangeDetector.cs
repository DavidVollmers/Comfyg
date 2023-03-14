using Comfyg.Client;
using Microsoft.Extensions.Primitives;

namespace Comfyg;

internal class ChangeDetector : IDisposable
{
    private readonly ComfygClient _client;
    private readonly TimeSpan _interval;
    private readonly Timer _timer;

    private CancellationTokenSource? _cancellationTokenSource;

    public ChangeDetector(ComfygClient client, TimeSpan interval)
    {
        _client = client;
        _interval = interval;

        _timer = new Timer(DetectChanges!, null, TimeSpan.Zero, interval);
    }

    public IChangeToken GetChangeToken()
    {
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();

        return new CancellationChangeToken(_cancellationTokenSource.Token);
    }

    private void DetectChanges(object state)
    {
        var since = DateTime.UtcNow.Add(-_interval);
        var result = _client.GetConfigurationDiffAsync(since).GetAwaiter().GetResult();
        if (result.ChangeLog.Any())
        {
            _cancellationTokenSource?.Cancel();
        }
    }

    public void Dispose()
    {
        _client.Dispose();
        _timer.Dispose();
        _cancellationTokenSource?.Dispose();
    }
}