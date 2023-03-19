using Comfyg.Client.Operations;
using Comfyg.Contracts;
using Comfyg.Timing;
using Microsoft.Extensions.Primitives;

namespace Comfyg;

internal class ChangeDetector<T> : IDisposable where T : IComfygValue
{
    private readonly IComfygValuesOperations<T> _operations;
    private readonly ITimer _timer;

    private CancellationTokenSource? _cancellationTokenSource;
    
    public DateTime LastDetectionAt { get; private set; }

    public ChangeDetector(IComfygValuesOperations<T> operations, ITimer timer)
    {
        _operations = operations ?? throw new ArgumentNullException(nameof(operations));
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
        var result = _operations.GetDiffAsync(LastDetectionAt).GetAwaiter().GetResult();
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