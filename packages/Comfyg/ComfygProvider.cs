using Comfyg.Client.Operations;
using Comfyg.Store.Contracts;
using Comfyg.Timing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Comfyg;

internal class ComfygProvider<T> : ConfigurationProvider, IDisposable where T : IComfygValue
{
    private readonly IComfygValueOperations<T> _operations;
    private readonly ChangeDetector<T>? _changeDetector;
    private readonly string[] _tags;

    public ComfygProvider(IComfygValueOperations<T> operations, ITimer? timer, IEnumerable<string>? tags)
    {
        _operations = operations ?? throw new ArgumentNullException(nameof(operations));
        _tags = tags?.ToArray() ?? throw new ArgumentNullException(nameof(tags)); 

        if (timer == null) return;
        _changeDetector = new ChangeDetector<T>(_operations, timer);
        ChangeToken.OnChange(_changeDetector.GetChangeToken,
            () => LoadAsync(_changeDetector.LastDetectionAt).GetAwaiter().GetResult());
    }

    public override void Load() => LoadAsync().GetAwaiter().GetResult();

    private async Task LoadAsync(DateTimeOffset? since = null)
    {
        try
        {
            var values = _operations.GetValuesAsync(since, _tags);

            if (!since.HasValue) Data.Clear();

            await foreach (var value in values.ConfigureAwait(false))
            {
                Set(value.Key, value.Value);
            }
        }
        catch
        {
            //TODO log exception
        }
    }

    public void Dispose()
    {
        _changeDetector?.Dispose();
        _operations.Dispose();
    }
}
