using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Comfyg.Store.Contracts.Configuration;
using Comfyg.Store.Contracts.Requests;

namespace Comfyg.Client.Operations;

internal class ConfigurationValuesOperations : IComfygValueOperations<IConfigurationValue>
{
    private readonly ComfygClient _client;

    public ConfigurationValuesOperations(ComfygClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async IAsyncEnumerable<IConfigurationValue> GetValuesAsync(DateTimeOffset? since = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var uri = "configuration";
        if (since.HasValue) uri += $"?since={since.Value.ToUniversalTime():s}Z";

        var response = await _client
            .SendRequestAsync(() => new HttpRequestMessage(HttpMethod.Get, uri), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to get configuration values.", null,
                response.StatusCode);

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        var values =
            JsonSerializer.DeserializeAsyncEnumerable<ConfigurationValue>(stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken);

        await foreach (var value in values.WithCancellation(cancellationToken).ConfigureAwait(false))
            yield return value!;
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

    public void Dispose()
    {
        _client.Dispose();
    }
}
