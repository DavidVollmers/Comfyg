using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Web;
using Comfyg.Client.Requests;
using Comfyg.Store.Contracts;

namespace Comfyg.Client.Operations;

internal class ConfigurationValuesOperations : IComfygValueOperations<IConfigurationValue>
{
    private readonly ComfygClient _client;

    public ConfigurationValuesOperations(ComfygClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async IAsyncEnumerable<IConfigurationValue> GetValuesAsync(DateTimeOffset? since = null,
        IEnumerable<string>? tags = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var uri = "configuration";

        var queryParameters = new List<string>();
        if (since.HasValue) queryParameters.Add($"since={since.Value.ToUniversalTime():s}Z");
        if (tags != null) queryParameters.AddRange(tags.Select(t => $"tags={HttpUtility.UrlEncode(t)}"));
        if (queryParameters.Any()) uri += "?" + string.Join('&', queryParameters);

        var response = await _client
            .SendRequestAsync(() => new HttpRequestMessage(HttpMethod.Get, uri), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to get configuration values.", null,
                response.StatusCode);

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        var values =
            JsonSerializer.DeserializeAsyncEnumerable<IConfigurationValue>(stream,
                new JsonSerializerOptions {PropertyNameCaseInsensitive = true}, cancellationToken);

        await foreach (var value in values.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            if (!_client.IsEncryptionEnabled)
            {
                yield return value!;
                continue;
            }

            yield return await _client
                .DecryptAsync<IConfigurationValue, ConfigurationValue.Initializer>(value!, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    public async Task<IConfigurationValue> GetValueAsync(string key, string? version = null,
        CancellationToken cancellationToken = default)
    {
        var uri = $"configuration/{HttpUtility.UrlEncode(key)}";
        if (version != null) uri += $"/{HttpUtility.UrlEncode(version)}";

        var response = await _client
            .SendRequestAsync(() => new HttpRequestMessage(HttpMethod.Get, uri), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to get configuration value.", null,
                response.StatusCode);

        var value = (await response.Content.ReadFromJsonAsync<IConfigurationValue>(cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;

        return _client.IsEncryptionEnabled
            ? await _client.DecryptAsync<IConfigurationValue, ConfigurationValue.Initializer>(value, cancellationToken)
                .ConfigureAwait(false)
            : value;
    }

    public async Task AddValuesAsync(IEnumerable<IConfigurationValue> values,
        CancellationToken cancellationToken = default)
    {
        if (values == null) throw new ArgumentNullException(nameof(values));

        if (_client.IsEncryptionEnabled)
        {
            values = await _client.EncryptAsync<IConfigurationValue, ConfigurationValue.Initializer>(values,
                cancellationToken).ConfigureAwait(false);
        }

        var response = await _client
            .SendRequestAsync(
                () => new HttpRequestMessage(HttpMethod.Post, "configuration")
                {
                    Content = JsonContent.Create(new AddConfigurationValuesRequest(values))
                }, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to add configuration values.", null,
                response.StatusCode);
    }

    public async Task<IConfigurationValue> TagValueAsync(string key, string tag,
        string version = ComfygConstants.LatestVersion, CancellationToken cancellationToken = default)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (tag == null) throw new ArgumentNullException(nameof(tag));
        if (version == null) throw new ArgumentNullException(nameof(version));

        var response = await _client
            .SendRequestAsync(
                () => new HttpRequestMessage(HttpMethod.Post, "configuration/tags")
                {
                    Content = JsonContent.Create(new TagValueRequest(key, tag, version))
                }, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to tag configuration value.", null,
                response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<IConfigurationValue>(cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
