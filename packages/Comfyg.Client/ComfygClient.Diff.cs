using System.Net.Http.Headers;
using System.Net.Http.Json;
using Comfyg.Contracts.Responses;

namespace Comfyg.Client;

public partial class ComfygClient
{
    public async Task<GetDiffResponse> GetConfigurationDiffAsync(DateTime since,
        CancellationToken cancellationToken = default)
    {
        var token = CreateToken();

        var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"diff/configuration?since={since:s}");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to get configuration diff.", null,
                response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<GetDiffResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;
    }
}