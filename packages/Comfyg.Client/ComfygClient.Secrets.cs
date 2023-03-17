using System.Net.Http.Headers;
using System.Net.Http.Json;
using Comfyg.Contracts.Requests;
using Comfyg.Contracts.Responses;

namespace Comfyg.Client;

public partial class ComfygClient
{
    public async Task<GetSecretValuesResponse> GetSecretValuesAsync(
        CancellationToken cancellationToken = default)
    {
        var token = CreateToken();

        var httpRequest = new HttpRequestMessage(HttpMethod.Get, "secrets");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to get secret values.", null,
                response.StatusCode);

        return (await response.Content
            .ReadFromJsonAsync<GetSecretValuesResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;
    }

    public async Task<GetSecretValuesResponse> GetSecretValuesFromDiffAsync(DateTime since,
        CancellationToken cancellationToken = default)
    {
        var token = CreateToken();

        var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"secrets/fromDiff?since={since:s}");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to get secret values from diff.",
                null, response.StatusCode);

        return (await response.Content
            .ReadFromJsonAsync<GetSecretValuesResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;
    }

    public async Task AddSecretValuesAsync(AddSecretValuesRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        var token = CreateToken();

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "secrets")
        {
            Content = JsonContent.Create(request)
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to add secret values.", null,
                response.StatusCode);
    }
}