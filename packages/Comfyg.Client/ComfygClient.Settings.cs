using System.Net.Http.Headers;
using System.Net.Http.Json;
using Comfyg.Contracts.Requests;
using Comfyg.Contracts.Responses;

namespace Comfyg.Client;

public partial class ComfygClient
{
    public async Task<GetSettingValuesResponse> GetSettingValuesAsync(
        CancellationToken cancellationToken = default)
    {
        var token = CreateToken();

        var httpRequest = new HttpRequestMessage(HttpMethod.Get, "settings");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to get setting values.", null,
                response.StatusCode);

        return (await response.Content
            .ReadFromJsonAsync<GetSettingValuesResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;
    }

    public async Task<GetSettingValuesResponse> GetSettingValuesFromDiffAsync(DateTime since,
        CancellationToken cancellationToken = default)
    {
        var token = CreateToken();

        var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"settings/fromDiff?since={since:s}");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to get setting values from diff.",
                null, response.StatusCode);

        return (await response.Content
            .ReadFromJsonAsync<GetSettingValuesResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;
    }

    public async Task AddSettingValuesAsync(AddSettingValuesRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        var token = CreateToken();

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "settings")
        {
            Content = JsonContent.Create(request)
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to add setting values.", null,
                response.StatusCode);
    }
}