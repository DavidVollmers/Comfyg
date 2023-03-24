using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Comfyg.Contracts.Changes;
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

    public async IAsyncEnumerable<IConfigurationValue> GetValuesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _client
            .SendRequestAsync(() => new HttpRequestMessage(HttpMethod.Get, "configuration"),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to get configuration values.", null,
                response.StatusCode);

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        var values =
            JsonSerializer.DeserializeAsyncEnumerable<ConfigurationValue>(stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken);

        await foreach (var value in values.ConfigureAwait(false)) yield return value!;
    }

    public async IAsyncEnumerable<IConfigurationValue> GetValuesFromDiffAsync(DateTimeOffset since,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _client
            .SendRequestAsync(
                () => new HttpRequestMessage(HttpMethod.Get,
                    $"configuration/fromDiff?since={since.ToUniversalTime():s}Z"),
                cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to get configuration values from diff.",
                null, response.StatusCode);

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        var values =
            JsonSerializer.DeserializeAsyncEnumerable<ConfigurationValue>(stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken);

        await foreach (var value in values.ConfigureAwait(false)) yield return value!;
    }

    public async Task AddValuesAsync(AddValuesRequest<IConfigurationValue> request,
        CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        var response = await _client
            .SendRequestAsync(
                () => new HttpRequestMessage(HttpMethod.Post, "configuration")
                {
                    Content = JsonContent.Create((AddConfigurationValuesRequest)request)
                }, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to add configuration values.", null,
                response.StatusCode);
    }

    public async IAsyncEnumerable<IChangeLog> GetDiffAsync(DateTimeOffset since,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _client
            .SendRequestAsync(
                () => new HttpRequestMessage(HttpMethod.Get, $"diff/configuration?since={since.ToUniversalTime():s}Z"),
                cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to get configuration diff.", null,
                response.StatusCode);

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        var values =
            JsonSerializer.DeserializeAsyncEnumerable<ChangeLog>(stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken);

        await foreach (var value in values.ConfigureAwait(false)) yield return value!;
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
