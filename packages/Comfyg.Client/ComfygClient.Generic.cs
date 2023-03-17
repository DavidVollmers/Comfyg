using Comfyg.Contracts;
using Comfyg.Contracts.Responses;

namespace Comfyg.Client;

public partial class ComfygClient
{
    public async Task<GetValuesResponse<T>> GetValuesAsync<T>(CancellationToken cancellationToken = default)
        where T : IComfygValue
    {
    }

    public async Task<GetValuesResponse<T>> GetValuesFromDiffAsync<T>(DateTime since,
        CancellationToken cancellationToken = default) where T : IComfygValue
    {
    }

    public async Task AddValuesAsync(AddValuesRequest request, CancellationToken cancellationToken = default)
    {
    }
}