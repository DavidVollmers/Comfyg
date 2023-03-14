using Comfyg.Client;
using Comfyg.Timing;
using Microsoft.Extensions.Primitives;

namespace Comfyg;

internal class ChangeDetector : IDisposable
{
    private readonly ComfygClient _client;
    private readonly ITimer _timer;

    private CancellationTokenSource? _cancellationTokenSource;

    public ChangeDetector(ComfygClient client, ITimer timer)
    {
        _client = client;
        _timer = timer;

        _timer.RegisterCallback(DetectChanges);
    }

    public IChangeToken GetChangeToken()
    {
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();

        return new CancellationChangeToken(_cancellationTokenSource.Token);
    }

    private void DetectChanges()
    {
        var since = DateTime.UtcNow.Add(-_timer.Interval);
        var result = _client.GetConfigurationDiffAsync(since).GetAwaiter().GetResult();
        if (result.ChangeLog.Any())
        {
            _cancellationTokenSource?.Cancel();
        }
    }

    public void Dispose()
    {
        _timer.Dispose();
        _cancellationTokenSource?.Dispose();
    }
}