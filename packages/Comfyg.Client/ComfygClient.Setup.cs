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
    /// <param name="publicKey">Can be used for asymmetric client secrets. Must contain the public key portion of the client secret.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> controlling the request lifespan.</param>
    /// <returns><see cref="ISetupClientResponse"/></returns>
    /// <exception cref="ArgumentNullException"><paramref name="client"/> is null.</exception>
    /// <exception cref="HttpRequestException">Invalid status code is returned.</exception>
    public async Task<ISetupClientResponse> SetupClientAsync(IClient client, Stream? publicKey = null,
        CancellationToken cancellationToken = default)
    {
        if (client == null) throw new ArgumentNullException(nameof(client));

        using var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(client.ClientId), nameof(ISetupClientRequest.ClientId));
        formData.Add(new StringContent(client.FriendlyName), nameof(ISetupClientRequest.FriendlyName));

        if (publicKey != null)
        {
            formData.Add(new StreamContent(publicKey), nameof(ISetupClientRequest.ClientSecretPublicKey),
                nameof(ISetupClientRequest.ClientSecretPublicKey));

            //TODO add private key encrypted encryption key
        }

        var response =
            await SendRequestAsync(
                // ReSharper disable once AccessToDisposedClosure
                () => new HttpRequestMessage(HttpMethod.Post, "setup/client") {Content = formData},
                cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to setup client.", null,
                response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<ISetupClientResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;
    }
}
