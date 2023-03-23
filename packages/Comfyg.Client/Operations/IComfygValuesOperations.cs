using Comfyg.Contracts;
using Comfyg.Contracts.Requests;
using Comfyg.Contracts.Responses;

namespace Comfyg.Client.Operations;

public interface IComfygValuesOperations<T> : IDisposable where T : IComfygValue
{
    Task<GetValuesResponse<T>> GetValuesAsync(CancellationToken cancellationToken = default);

    Task<GetValuesResponse<T>> GetValuesFromDiffAsync(DateTimeOffset since,
        CancellationToken cancellationToken = default);

    Task AddValuesAsync(AddValuesRequest<T> request, CancellationToken cancellationToken = default);

    Task<GetDiffResponse> GetDiffAsync(DateTimeOffset since, CancellationToken cancellationToken = default);
}