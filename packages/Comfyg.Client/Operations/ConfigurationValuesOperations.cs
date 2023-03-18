using System.Net.Http.Json;
using Comfyg.Contracts.Configuration;
using Comfyg.Contracts.Requests;
using Comfyg.Contracts.Responses;

namespace Comfyg.Client.Operations;

internal class ConfigurationValuesOperations : IComfygValuesOperations<IConfigurationValue>
{
    private readonly ComfygClient _client;

    public ConfigurationValuesOperations(ComfygClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<GetValuesResponse<IConfigurationValue>> GetValuesAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _client
            .SendRequestAsync(new HttpRequestMessage(HttpMethod.Get, "configuration"), cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to get configuration values.", null,
                response.StatusCode);

        return (await response.Content
            .ReadFromJsonAsync<GetConfigurationValuesResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;
    }

    public async Task<GetValuesResponse<IConfigurationValue>> GetValuesFromDiffAsync(DateTime since,
        CancellationToken cancellationToken = default)
    {
        var response = await _client
            .SendRequestAsync(new HttpRequestMessage(HttpMethod.Get, $"configuration/fromDiff?since={since:s}"),
                cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to get configuration values from diff.",
                null, response.StatusCode);

        return (await response.Content
            .ReadFromJsonAsync<GetConfigurationValuesResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;
    }

    public async Task AddValuesAsync(AddValuesRequest<IConfigurationValue> request,
        CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        var response = await _client.SendRequestAsync(new HttpRequestMessage(HttpMethod.Post, "configuration")
        {
            Content = JsonContent.Create(request)
        }, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to add configuration values.", null,
                response.StatusCode);
    }

    public async Task<GetDiffResponse> GetDiffAsync(DateTime since, CancellationToken cancellationToken = default)
    {
        var response = await _client
            .SendRequestAsync(new HttpRequestMessage(HttpMethod.Get, $"diff/configuration?since={since:s}"),
                cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to get configuration diff.", null,
                response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<GetDiffResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}