using Comfyg.Client.Operations;
using Comfyg.Contracts;
using Comfyg.Contracts.Configuration;
using Comfyg.Contracts.Requests;
using Comfyg.Contracts.Responses;
using Comfyg.Contracts.Secrets;
using Comfyg.Contracts.Settings;

namespace Comfyg.Client;

public partial class ComfygClient
{
    public IComfygValuesOperations<T> Operations<T>() where T : IComfygValue
    {
        if (typeof(T) == typeof(IConfigurationValue)) return (IComfygValuesOperations<T>)Configuration;
        if (typeof(T) == typeof(ISettingValue)) return (IComfygValuesOperations<T>)Settings;
        if (typeof(T) == typeof(ISecretValue)) return (IComfygValuesOperations<T>)Secrets;
        throw new NotSupportedException();
    }

    public async Task<GetValuesResponse<T>> GetValuesAsync<T>(CancellationToken cancellationToken = default)
        where T : IComfygValue
    {
        return await Operations<T>().GetValuesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<GetValuesResponse<T>> GetValuesFromDiffAsync<T>(DateTime since,
        CancellationToken cancellationToken = default) where T : IComfygValue
    {
        return await Operations<T>().GetValuesFromDiffAsync(since, cancellationToken).ConfigureAwait(false);
    }

    public async Task AddValuesAsync<T>(AddValuesRequest<T> request, CancellationToken cancellationToken = default)
        where T : IComfygValue
    {
        await Operations<T>().AddValuesAsync(request, cancellationToken).ConfigureAwait(false);
    }
}