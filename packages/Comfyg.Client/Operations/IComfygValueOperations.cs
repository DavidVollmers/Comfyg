using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Requests;

namespace Comfyg.Client.Operations;

/// <summary>
/// Generic Comfyg value operations used to manage values of the specific type.
/// </summary>
/// <typeparam name="T">The type of the values to manage.</typeparam>
public interface IComfygValueOperations<T> : IDisposable where T : IComfygValue
{
    /// <summary>
    /// Retrieves values from the connected Comfyg store.
    /// </summary>
    /// <param name="since">If provided, only values which were created or edited afterwards are retrieved.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> controlling the request lifespan.</param>
    /// <typeparam name="T">The type of the values to retrieve.</typeparam>
    /// <returns><see cref="IAsyncEnumerable{T}"/></returns>
    IAsyncEnumerable<T> GetValuesAsync(DateTimeOffset? since = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds values of the specific type to the connected Comfyg store.
    /// </summary>
    /// <param name="request"><see cref="AddValuesRequest{T}"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> controlling the request lifespan.</param>
    /// <typeparam name="T">The type of the values to add.</typeparam>
    Task AddValuesAsync(AddValuesRequest<T> request, CancellationToken cancellationToken = default);
}
