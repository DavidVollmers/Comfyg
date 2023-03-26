using System.Net.Http.Headers;
using System.Net.Http.Json;
using Comfyg.Store.Contracts.Requests;
using Comfyg.Store.Contracts.Responses;

namespace Comfyg.Client;

public partial class ComfygClient
{
    public async Task<SetupClientResponse> SetupClientAsync(SetupClientRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        var token = CreateToken();

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "setup/client")
        {
            Content = JsonContent.Create(request)
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to setup client.", null,
                response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<SetupClientResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;
    }
}
