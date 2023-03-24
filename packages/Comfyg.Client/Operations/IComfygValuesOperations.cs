using Comfyg.Contracts;
using Comfyg.Contracts.Changes;
using Comfyg.Contracts.Requests;

namespace Comfyg.Client.Operations;

public interface IComfygValuesOperations<T> : IDisposable where T : IComfygValue
{
    IAsyncEnumerable<T> GetValuesAsync(CancellationToken cancellationToken = default);

    IAsyncEnumerable<T> GetValuesFromDiffAsync(DateTimeOffset since, CancellationToken cancellationToken = default);

    Task AddValuesAsync(AddValuesRequest<T> request, CancellationToken cancellationToken = default);

    IAsyncEnumerable<IChangeLog> GetDiffAsync(DateTimeOffset since, CancellationToken cancellationToken = default);
}
