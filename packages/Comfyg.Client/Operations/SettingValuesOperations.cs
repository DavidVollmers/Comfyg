using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Comfyg.Client.Requests;
using Comfyg.Store.Contracts;

namespace Comfyg.Client.Operations;

internal class SettingValuesOperations : IComfygValueOperations<ISettingValue>
{
    private readonly ComfygClient _client;

    public SettingValuesOperations(ComfygClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async IAsyncEnumerable<ISettingValue> GetValuesAsync(DateTimeOffset? since = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var uri = "settings";
        if (since.HasValue) uri += $"?since={since.Value.ToUniversalTime():s}Z";

        var response = await _client
            .SendRequestAsync(() => new HttpRequestMessage(HttpMethod.Get, uri), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to get setting values.", null,
                response.StatusCode);

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        var values =
            JsonSerializer.DeserializeAsyncEnumerable<SettingValue>(stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken);

        await foreach (var value in values.WithCancellation(cancellationToken).ConfigureAwait(false))
            yield return value!;
    }

    public async Task AddValuesAsync(IEnumerable<ISettingValue> values, CancellationToken cancellationToken = default)
    {
        if (values == null) throw new ArgumentNullException(nameof(values));

        var response = await _client
            .SendRequestAsync(
                () => new HttpRequestMessage(HttpMethod.Post, "settings")
                {
                    Content = JsonContent.Create(new AddSettingValuesRequest(values))
                }, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to add setting values.", null,
                response.StatusCode);
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
