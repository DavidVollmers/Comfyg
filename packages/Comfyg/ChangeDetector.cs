using Comfyg.Client.Operations;
using Comfyg.Contracts;
using Comfyg.Timing;
using Microsoft.Extensions.Primitives;

namespace Comfyg;

internal class ChangeDetector<T> : IDisposable where T : IComfygValue
{
    private readonly IComfygValueOperations<T> _operations;
    private readonly ITimer _timer;

    private CancellationTokenSource? _cancellationTokenSource;

    public DateTimeOffset LastDetectionAt { get; private set; }

    public ChangeDetector(IComfygValueOperations<T> operations, ITimer timer)
    {
        _operations = operations ?? throw new ArgumentNullException(nameof(operations));
        _timer = timer ?? throw new ArgumentNullException(nameof(timer));

        _timer.RegisterCallback(() => DetectChangesAsync().GetAwaiter().GetResult());
    }

    public IChangeToken GetChangeToken()
    {
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();

        return new CancellationChangeToken(_cancellationTokenSource.Token);
    }

    private async Task DetectChangesAsync()
    {
        try
        {
            LastDetectionAt = DateTimeOffset.UtcNow.Add(-_timer.Interval);

            var cancellationToken = _cancellationTokenSource!.Token;
            var changes = _operations.GetValuesAsync(LastDetectionAt, cancellationToken);
            await foreach (var _ in changes.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                _cancellationTokenSource?.Cancel();
                break;
            }
        }
        catch
        {
            //TODO log exception
        }
    }

    public void Dispose()
    {
        _timer.Dispose();
        _cancellationTokenSource?.Dispose();
    }
}
