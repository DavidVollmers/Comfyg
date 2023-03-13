using System.Net.Http.Headers;
using System.Net.Http.Json;
using Comfyg.Contracts.Requests;

namespace Comfyg.Client;

public partial class ComfygClient
{
    public async Task AddConfigurationAsync(AddConfigurationRequest request,
        CancellationToken cancellationToken = default)
    {
        var token = CreateToken();

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "configuration")
        {
            Content = JsonContent.Create(request)
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to add configuration", null,
                response.StatusCode);
    }
}