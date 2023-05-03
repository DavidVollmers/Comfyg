using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Web;
using Comfyg.Client.Requests;
using Comfyg.Store.Contracts;

namespace Comfyg.Client.Operations;

internal class SecretValuesOperations : IComfygValueOperations<ISecretValue>
{
    private readonly ComfygClient _client;

    public SecretValuesOperations(ComfygClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async IAsyncEnumerable<ISecretValue> GetValuesAsync(DateTimeOffset? since = null,
        IEnumerable<string>? tags = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var uri = "secrets";

        var queryParameters = new List<string>();
        if (since.HasValue) queryParameters.Add($"since={since.Value.ToUniversalTime():s}Z");
        if (tags != null) queryParameters.AddRange(tags.Select(t => $"tags={HttpUtility.UrlEncode(t)}"));
        if (queryParameters.Any()) uri += "?" + string.Join('&', queryParameters);

        var response = await _client
            .SendRequestAsync(() => new HttpRequestMessage(HttpMethod.Get, uri), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to get secret values.", null,
                response.StatusCode);

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        var values =
            JsonSerializer.DeserializeAsyncEnumerable<ISecretValue>(stream,
                new JsonSerializerOptions {PropertyNameCaseInsensitive = true}, cancellationToken);

        await foreach (var value in values.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            if (!_client.IsEncryptionEnabled) yield return value!;

            yield return await _client.DecryptAsync<ISecretValue, SecretValue.Initializer>(value!, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    public async Task<ISecretValue> GetValueAsync(string key, string? version = null,
        CancellationToken cancellationToken = default)
    {
        var uri = $"secrets/{HttpUtility.UrlEncode(key)}";
        if (version != null) uri += $"/{HttpUtility.UrlEncode(version)}";

        var response = await _client
            .SendRequestAsync(() => new HttpRequestMessage(HttpMethod.Get, uri), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to get secret value.", null,
                response.StatusCode);

        var value = (await response.Content.ReadFromJsonAsync<ISecretValue>(cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;

        return !_client.IsEncryptionEnabled
            ? value
            : await _client.DecryptAsync<ISecretValue, SecretValue.Initializer>(value, cancellationToken)
                .ConfigureAwait(false);
    }

    public async Task AddValuesAsync(IEnumerable<ISecretValue> values, CancellationToken cancellationToken = default)
    {
        if (values == null) throw new ArgumentNullException(nameof(values));

        if (_client.IsEncryptionEnabled)
        {
            values = await _client.EncryptAsync<ISecretValue, SecretValue.Initializer>(values, cancellationToken)
                .ConfigureAwait(false);
        }

        var response = await _client
            .SendRequestAsync(
                () => new HttpRequestMessage(HttpMethod.Post, "secrets")
                {
                    Content = JsonContent.Create(new AddSecretValuesRequest(values))
                }, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to add secret values.", null,
                response.StatusCode);
    }

    public async Task<ISecretValue> TagValueAsync(string key, string tag,
        string version = ComfygConstants.LatestVersion, CancellationToken cancellationToken = default)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (tag == null) throw new ArgumentNullException(nameof(tag));
        if (version == null) throw new ArgumentNullException(nameof(version));

        var response = await _client
            .SendRequestAsync(
                () => new HttpRequestMessage(HttpMethod.Post, "secrets/tags")
                {
                    Content = JsonContent.Create(new TagValueRequest(key, tag, version))
                }, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to tag secret value.", null,
                response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<ISecretValue>(cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
