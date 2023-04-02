using System.Net.Http.Json;
using Comfyg.Client.Requests;
using Comfyg.Store.Contracts;

namespace Comfyg.Client;

public partial class ComfygClient
{
    /// <summary>
    /// Sets permission on a Comfyg value for a specific client.
    /// </summary>
    /// <param name="clientId">The ID of the client to set the permission for.</param>
    /// <param name="key">The key of the Comfyg value to set the permission for.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> controlling the request lifespan.</param>
    /// <typeparam name="T">The type of the Comfyg value.</typeparam>
    /// <exception cref="ArgumentNullException"><paramref name="clientId"/> or <paramref name="key"/> is null.</exception>
    /// <exception cref="HttpRequestException">Invalid status code is returned.</exception>
    /// <exception cref="NotSupportedException"><typeparamref name="T"/> is not supported.</exception>
    public async Task SetPermissionAsync<T>(string clientId, string key, CancellationToken cancellationToken = default)
        where T : IComfygValue
    {
        if (clientId == null) throw new ArgumentNullException(nameof(clientId));
        if (key == null) throw new ArgumentNullException(nameof(key));

        var uri = "permissions/";
        if (typeof(T) == typeof(IConfigurationValue)) uri += "configuration";
        else if (typeof(T) == typeof(ISecretValue)) uri += "secrets";
        else if (typeof(T) == typeof(ISettingValue)) uri += "settings";
        else throw new NotSupportedException();
        
        var response = await SendRequestAsync(
            () => new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(new[] { new SetPermissionRequest(clientId, key) })
            },
            cancellationToken: cancellationToken);
        
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to set permissions.", null,
                response.StatusCode);
    }
}
