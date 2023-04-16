using Comfyg.Store.Contracts;

namespace Comfyg.Store.Core.Abstractions;

public interface IValueService<T> where T : IComfygValue
{
    Task AddValueAsync(string owner, string key, string value, string hash,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<T> GetValuesAsync(string owner, CancellationToken cancellationToken = default);

    Task<T?> GetValueAsync(string key, string version, CancellationToken cancellationToken = default);

    Task<T?> GetLatestValueAsync(string key, CancellationToken cancellationToken = default);

    Task<T> TagValueAsync(string owner, string key, string tag, string version,
        CancellationToken cancellationToken = default);
}
