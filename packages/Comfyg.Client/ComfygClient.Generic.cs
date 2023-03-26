using System.Runtime.CompilerServices;
using Comfyg.Client.Operations;
using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Configuration;
using Comfyg.Store.Contracts.Requests;
using Comfyg.Store.Contracts.Secrets;
using Comfyg.Store.Contracts.Settings;

namespace Comfyg.Client;

public partial class ComfygClient
{
    public IComfygValueOperations<T> Operations<T>() where T : IComfygValue
    {
        if (typeof(T) == typeof(IConfigurationValue)) return (IComfygValueOperations<T>)Configuration;
        if (typeof(T) == typeof(ISettingValue)) return (IComfygValueOperations<T>)Settings;
        if (typeof(T) == typeof(ISecretValue)) return (IComfygValueOperations<T>)Secrets;
        throw new NotSupportedException();
    }

    public async IAsyncEnumerable<T> GetValuesAsync<T>(DateTimeOffset? since = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default) where T : IComfygValue
    {
        var values = Operations<T>().GetValuesAsync(since, cancellationToken);

        await foreach (var value in values.WithCancellation(cancellationToken).ConfigureAwait(false))
            yield return value;
    }

    public async Task AddValuesAsync<T>(AddValuesRequest<T> request, CancellationToken cancellationToken = default)
        where T : IComfygValue
    {
        await Operations<T>().AddValuesAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
