using Comfyg.Client;
using Comfyg.Timing;
using Microsoft.Extensions.Primitives;

namespace Comfyg;

internal class ChangeDetector : IDisposable
{
    private readonly ComfygClient _client;
    private readonly ITimer _timer;

    private CancellationTokenSource? _cancellationTokenSource;
    
    public DateTime LastDetectionAt { get; private set; }

    public ChangeDetector(ComfygClient client, ITimer timer)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _timer = timer ?? throw new ArgumentNullException(nameof(timer));

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
        LastDetectionAt = DateTime.UtcNow.Add(-_timer.Interval);
        var result = _client.GetConfigurationDiffAsync(LastDetectionAt).GetAwaiter().GetResult();
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