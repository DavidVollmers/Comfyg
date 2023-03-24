using Comfyg.Contracts;
using Comfyg.Contracts.Requests;

namespace Comfyg.Client.Operations;

public interface IComfygValuesOperations<T> : IDisposable where T : IComfygValue
{
    IAsyncEnumerable<T> GetValuesAsync(DateTimeOffset? since = null, CancellationToken cancellationToken = default);

    Task AddValuesAsync(AddValuesRequest<T> request, CancellationToken cancellationToken = default);
}
