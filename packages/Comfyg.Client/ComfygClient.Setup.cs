using System.Net.Http.Json;
using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Requests;
using Comfyg.Store.Contracts.Responses;

namespace Comfyg.Client;

public partial class ComfygClient
{
    /// <summary>
    /// Registers a new client on the connected Comfyg store. 
    /// </summary>
    /// <param name="client"><see cref="IClient"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> controlling the request lifespan.</param>
    /// <returns><see cref="ISetupClientResponse"/></returns>
    /// <exception cref="ArgumentNullException"><paramref name="client"/> is null.</exception>
    /// <exception cref="HttpRequestException">Invalid status code is returned.</exception>
    public async Task<ISetupClientResponse> SetupClientAsync(IClient client,
        CancellationToken cancellationToken = default)
    {
        if (client == null) throw new ArgumentNullException(nameof(client));

        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(client.ClientId),
            $"{nameof(ISetupClientRequest)}.{nameof(ISetupClientRequest.ClientId)}");
        formData.Add(new StringContent(client.FriendlyName),
            $"{nameof(ISetupClientRequest)}.{nameof(ISetupClientRequest.FriendlyName)}");

        var response =
            await SendRequestAsync(
                () => new HttpRequestMessage(HttpMethod.Post, "setup/client") { Content = formData },
                cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to setup client.", null,
                response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<ISetupClientResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;
    }
}
