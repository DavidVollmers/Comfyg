using Azure.Security.KeyVault.Secrets;
using Comfyg.Store.Core.Abstractions.Secrets;

namespace Comfyg.Store.Core.Secrets;

public sealed class KeyVaultSecretService : ISecretService
{
    private const string SecretNameDelimiter = "--";
    private const int MaxSecretNameGenerationTries = 10;

    private readonly string _systemId;
    private readonly SecretClient _client;

    public KeyVaultSecretService(string systemId, SecretClient client)
    {
        _systemId = systemId ?? throw new ArgumentNullException(nameof(systemId));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<string> ProtectSecretValueAsync(string value, CancellationToken cancellationToken = default)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        string? name;
        var tries = 0;
        do
        {
            name = $"{_systemId}{SecretNameDelimiter}{Guid.NewGuid()}";

            var existing = await _client.GetSecretAsync(name, cancellationToken: cancellationToken);
            if (existing.HasValue) name = null;
        } while (name == null && ++tries > MaxSecretNameGenerationTries);

        if (name == null) throw new Exception("Could not generate unique key vault secret name.");

        var result = await _client.SetSecretAsync(name, value, cancellationToken);

        if (!result.HasValue) throw new Exception("Could not set key vault secret.");

        return name + SecretNameDelimiter + result.Value.Properties.Version;
    }

    public async Task<string> UnprotectSecretValueAsync(string value, CancellationToken cancellationToken = default)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        var parts = value.Split(SecretNameDelimiter);
        if (parts.Length != 3 || parts[0] != _systemId || !Guid.TryParse(parts[1], out var guid))
            throw new InvalidOperationException("Value is no key vault secret from this system.");

        var name = $"{_systemId}{SecretNameDelimiter}{guid}";

        var result = await _client.GetSecretAsync(name, parts[2], cancellationToken: cancellationToken);

        if (!result.HasValue) throw new Exception("Could not get key vault secret.");

        return result.Value.Value;
    }
}
