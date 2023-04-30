using System.Runtime.CompilerServices;
using Comfyg.Client.Operations;
using Comfyg.Store.Contracts;

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
        if (typeof(T) == typeof(ISecretValue)) return (IComfygValueOperations<T>)Secrets;
        if (typeof(T) == typeof(ISettingValue)) return (IComfygValueOperations<T>)Settings;
        throw new NotSupportedException();
    }

    /// <summary>
    /// Retrieves values from the connected Comfyg store.
    /// </summary>
    /// <param name="since">If provided, only values which were created or edited afterwards are retrieved.</param>
    /// <param name="tags">Can be used to filter all values by their tags. If no tag matches the original value will be returned, otherwise the last matching tag from the array is returned.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> controlling the request lifespan.</param>
    /// <typeparam name="T">The type of the values to retrieve.</typeparam>
    /// <returns><see cref="IAsyncEnumerable{T}"/></returns>
    public async IAsyncEnumerable<T> GetValuesAsync<T>(DateTimeOffset? since = null, IEnumerable<string>? tags = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default) where T : IComfygValue
    {
        var values = Operations<T>().GetValuesAsync(since, tags, cancellationToken).ConfigureAwait(false);

        await foreach (var value in values.WithCancellation(cancellationToken).ConfigureAwait(false))
            yield return value;
    }

    /// <summary>
    /// Retrieves a single value from the connected Comfyg store.
    /// </summary>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <param name="version">The version of the value to retrieve. If not provided the latest version will be retrieved.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> controlling the request lifespan.</param>
    /// <typeparam name="T">The type of the values to retrieve.</typeparam>
    /// <returns><see cref="T"/>></returns>
    public async Task<T> GetValueAsync<T>(string key, string? version = null,
        CancellationToken cancellationToken = default) where T : IComfygValue
    {
        return await Operations<T>().GetValueAsync(key, version, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Adds values of the specific type to the connected Comfyg store.
    /// </summary>
    /// <param name="values"><see cref="IEnumerable{T}"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> controlling the request lifespan.</param>
    /// <typeparam name="T">The type of the values to add.</typeparam>
    public async Task AddValuesAsync<T>(IEnumerable<T> values, CancellationToken cancellationToken = default)
        where T : IComfygValue
    {
        await Operations<T>().AddValuesAsync(values, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Tags a version of a specific value.
    /// </summary>
    /// <param name="key">The key of the value to tag.</param>
    /// <param name="tag">The identifier of the tag.</param>
    /// <param name="version">The version of the value to tag. Defaults to `latest`.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> controlling the request lifespan.</param>
    /// <typeparam name="T">The type of the values to add.</typeparam>
    public async Task<T> TagValueAsync<T>(string key, string tag, string version = ComfygConstants.LatestVersion,
        CancellationToken cancellationToken = default) where T : IComfygValue
    {
        return await Operations<T>().TagValueAsync(key, tag, version, cancellationToken).ConfigureAwait(false);
    }
}
