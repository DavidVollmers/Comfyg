using System.Net.Http.Json;
using System.Security.Cryptography;
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
    /// <param name="keys">Can be used for asymmetric client secrets. Must contain the public key portion of the client secret and the private key portion for end-to-end encryption.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> controlling the request lifespan.</param>
    /// <returns><see cref="ISetupClientResponse"/></returns>
    /// <exception cref="ArgumentNullException"><paramref name="client"/> is null.</exception>
    /// <exception cref="HttpRequestException">Invalid status code is returned.</exception>
    public async Task<ISetupClientResponse> SetupClientAsync(IClient client, RSA? keys = null,
        CancellationToken cancellationToken = default)
    {
        if (client == null) throw new ArgumentNullException(nameof(client));

        using var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(client.ClientId), nameof(ISetupClientRequest.ClientId));
        formData.Add(new StringContent(client.FriendlyName), nameof(ISetupClientRequest.FriendlyName));

        if (keys != null)
        {
            var publicKey = new MemoryStream(keys.ExportRSAPublicKey());
            formData.Add(new StreamContent(publicKey), nameof(ISetupClientRequest.ClientSecretPublicKey),
                nameof(ISetupClientRequest.ClientSecretPublicKey));

            var rawEncryptionKey = await GetEncryptionKeyAsync(cancellationToken);

            if (rawEncryptionKey == null)
            {
                using var aes = Aes.Create();
                rawEncryptionKey = aes.Key;
            }

            var encryptedKey = keys.Encrypt(rawEncryptionKey, RSAEncryptionPadding.Pkcs1);
            var encryptionKey = new MemoryStream(encryptedKey);
            formData.Add(new StreamContent(encryptionKey), nameof(ISetupClientRequest.EncryptionKey),
                nameof(ISetupClientRequest.EncryptionKey));
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
