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
    /// <summary>
    /// Provides methods to manage values of the specific type in the connected Comfyg store.
    /// </summary>
    /// <typeparam name="T">The type of the values to manage.</typeparam>
    /// <returns><see cref="IComfygValueOperations{T}"/></returns>
    /// <exception cref="NotSupportedException"><typeparamref name="T"/> is not supported.</exception>
    public IComfygValueOperations<T> Operations<T>() where T : IComfygValue
    {
        if (typeof(T) == typeof(IConfigurationValue)) return (IComfygValueOperations<T>)Configuration;
        if (typeof(T) == typeof(ISettingValue)) return (IComfygValueOperations<T>)Settings;
        if (typeof(T) == typeof(ISecretValue)) return (IComfygValueOperations<T>)Secrets;
        throw new NotSupportedException();
    }

    /// <summary>
    /// Retrieves values from the connected Comfyg store.
    /// </summary>
    /// <param name="since">If provided, only values which were created or edited afterwards are retrieved.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> controlling the request lifespan.</param>
    /// <typeparam name="T">The type of the values to retrieve.</typeparam>
    /// <returns><see cref="IAsyncEnumerable{T}"/></returns>
    public async IAsyncEnumerable<T> GetValuesAsync<T>(DateTimeOffset? since = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default) where T : IComfygValue
    {
        var values = Operations<T>().GetValuesAsync(since, cancellationToken);

        await foreach (var value in values.WithCancellation(cancellationToken).ConfigureAwait(false))
            yield return value;
    }

    /// <summary>
    /// Adds values of the specific type to the connected Comfyg store.
    /// </summary>
    /// <param name="request"><see cref="AddValuesRequest{T}"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> controlling the request lifespan.</param>
    /// <typeparam name="T">The type of the values to add.</typeparam>
    public async Task AddValuesAsync<T>(IAddValuesRequest<T> request, CancellationToken cancellationToken = default)
        where T : IComfygValue
    {
        await Operations<T>().AddValuesAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
