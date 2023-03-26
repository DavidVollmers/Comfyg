﻿using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Comfyg.Contracts.Requests;
using Comfyg.Contracts.Secrets;

namespace Comfyg.Client.Operations;

internal class SecretValuesOperations : IComfygValueOperations<ISecretValue>
{
    private readonly ComfygClient _client;

    public SecretValuesOperations(ComfygClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async IAsyncEnumerable<ISecretValue> GetValuesAsync(DateTimeOffset? since = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var uri = "secrets";
        if (since.HasValue) uri += $"?since={since.Value.ToUniversalTime():s}Z";

        var response = await _client
            .SendRequestAsync(() => new HttpRequestMessage(HttpMethod.Get, uri), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to get secret values.", null,
                response.StatusCode);

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        var values =
            JsonSerializer.DeserializeAsyncEnumerable<SecretValue>(stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken);

        await foreach (var value in values.WithCancellation(cancellationToken).ConfigureAwait(false))
            yield return value!;
    }

    public async Task AddValuesAsync(AddValuesRequest<ISecretValue> request,
        CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        var response = await _client
            .SendRequestAsync(
                () => new HttpRequestMessage(HttpMethod.Post, "secrets")
                {
                    Content = JsonContent.Create((AddSecretValuesRequest)request)
                }, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Invalid status code when trying to add secret values.", null,
                response.StatusCode);
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
