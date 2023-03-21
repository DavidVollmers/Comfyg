using System.Net.Http.Json;
using Comfyg.Contracts.Requests;
using Comfyg.Contracts.Responses;
using Comfyg.Contracts.Settings;

namespace Comfyg.Client.Operations;

internal class SettingValuesOperations : IComfygValuesOperations<ISettingValue>
{
    private readonly ComfygClient _client;

    public SettingValuesOperations(ComfygClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<GetValuesResponse<ISettingValue>> GetValuesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client
            .SendRequestAsync(() => new HttpRequestMessage(HttpMethod.Get, "settings"),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to get setting values.", null,
                response.StatusCode);

        return (await response.Content
            .ReadFromJsonAsync<GetSettingValuesResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;
    }

    public async Task<GetValuesResponse<ISettingValue>> GetValuesFromDiffAsync(DateTime since,
        CancellationToken cancellationToken = default)
    {
        var response = await _client
            .SendRequestAsync(() => new HttpRequestMessage(HttpMethod.Get, $"settings/fromDiff?since={since.ToUniversalTime():s}Z"),
                cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to get setting values from diff.",
                null, response.StatusCode);

        return (await response.Content
            .ReadFromJsonAsync<GetSettingValuesResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;
    }

    public async Task AddValuesAsync(AddValuesRequest<ISettingValue> request,
        CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        var response = await _client.SendRequestAsync(() => new HttpRequestMessage(HttpMethod.Post, "settings")
        {
            Content = JsonContent.Create((AddSettingValuesRequest)request)
        }, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to add setting values.", null,
                response.StatusCode);
    }

    public async Task<GetDiffResponse> GetDiffAsync(DateTime since, CancellationToken cancellationToken = default)
    {
        var response = await _client
            .SendRequestAsync(() => new HttpRequestMessage(HttpMethod.Get, $"diff/settings?since={since.ToUniversalTime():s}Z"),
                cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to get settings diff.", null,
                response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<GetDiffResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}