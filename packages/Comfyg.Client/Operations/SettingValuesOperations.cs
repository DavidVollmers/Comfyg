using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
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

    public async IAsyncEnumerable<ISettingValue> GetValuesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _client
            .SendRequestAsync(() => new HttpRequestMessage(HttpMethod.Get, "settings"),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to get setting values.", null,
                response.StatusCode);

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        var values =
            JsonSerializer.DeserializeAsyncEnumerable<SettingValue>(stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken);

        await foreach (var value in values.ConfigureAwait(false)) yield return value!;
    }

    public async IAsyncEnumerable<ISettingValue> GetValuesFromDiffAsync(DateTimeOffset since,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _client
            .SendRequestAsync(
                () => new HttpRequestMessage(HttpMethod.Get, $"settings/fromDiff?since={since.ToUniversalTime():s}Z"),
                cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to get setting values from diff.",
                null, response.StatusCode);

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        var values =
            JsonSerializer.DeserializeAsyncEnumerable<SettingValue>(stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken);

        await foreach (var value in values.ConfigureAwait(false)) yield return value!;
    }

    public async Task AddValuesAsync(AddValuesRequest<ISettingValue> request,
        CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        var response = await _client
            .SendRequestAsync(
                () => new HttpRequestMessage(HttpMethod.Post, "settings")
                {
                    Content = JsonContent.Create((AddSettingValuesRequest)request)
                }, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to add setting values.", null,
                response.StatusCode);
    }

    public async Task<GetDiffResponse> GetDiffAsync(DateTimeOffset since, CancellationToken cancellationToken = default)
    {
        var response = await _client
            .SendRequestAsync(
                () => new HttpRequestMessage(HttpMethod.Get, $"diff/settings?since={since.ToUniversalTime():s}Z"),
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
