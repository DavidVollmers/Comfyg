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
            var result = _operations.GetValuesAsync().GetAwaiter().GetResult();
            SetData(result.Values);
        }
        catch
        {
            //TODO log exception
        }
    }

    private void LoadDiff(DateTime since)
    {
        try
        {
            var result = _operations.GetValuesFromDiffAsync(since).GetAwaiter().GetResult();
            SetData(result.Values, false);
        }
        catch
        {
            //TODO log exception
        }
    }

    private void SetData(IEnumerable<T> values, bool reset = true)
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
        _operations.Dispose();
    }
}