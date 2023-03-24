using Comfyg.Client.Operations;
using Comfyg.Contracts;
using Comfyg.Timing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Comfyg;

internal class ComfygProvider<T> : ConfigurationProvider, IDisposable where T : IComfygValue
{
    private readonly IComfygValuesOperations<T> _operations;
    private readonly ChangeDetector<T>? _changeDetector;

    public ComfygProvider(IComfygValuesOperations<T> operations, ITimer? timer = null)
    {
        _operations = operations ?? throw new ArgumentNullException(nameof(operations));

        if (timer == null) return;
        _changeDetector = new ChangeDetector<T>(_operations, timer);
        ChangeToken.OnChange(_changeDetector.GetChangeToken, () => { LoadDiff(_changeDetector.LastDetectionAt); });
    }

    public override void Load()
    {
        try
        {
            var result = _operations.GetValuesAsync();
            SetDataAsync(result).GetAwaiter().GetResult();
        }
        catch
        {
            //TODO log exception
        }
    }

    private void LoadDiff(DateTimeOffset since)
    {
        try
        {
            var result = _operations.GetValuesFromDiffAsync(since);
            SetDataAsync(result, false).GetAwaiter().GetResult();
        }
        catch
        {
            //TODO log exception
        }
    }

    private async Task SetDataAsync(IAsyncEnumerable<T> values, bool reset = true)
    {
        if (reset) Data.Clear();

        await foreach (var value in values)
        {
            Set(value.Key, value.Value);
        }
    }

    public void Dispose()
    {
        _changeDetector?.Dispose();
        _operations.Dispose();
    }
}
