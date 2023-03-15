using Azure.Security.KeyVault.Secrets;
using Comfyg.Core.Abstractions.Changes;
using Comfyg.Core.Abstractions.Permissions;
using CoreHelpers.WindowsAzure.Storage.Table;

namespace Comfyg.Core.Secrets;

public sealed class KeyVaultSecretService : SecretServiceBase
{
    private const string SecretNameDelimiter = "--";
    private const int MaxSecretNameGenerationTries = 10;

    private readonly SecretClient _client;

    public KeyVaultSecretService(string systemId, SecretClient client, IStorageContext storageContext,
        IChangeService changeService, IPermissionService permissionService)
        : base(systemId, storageContext, changeService, permissionService)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override async Task<string> ProtectSecretValueAsync(string value,
        CancellationToken cancellationToken = default)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        string? name;
        var tries = 0;
        do
        {
            name = $"{SystemId}{SecretNameDelimiter}{Guid.NewGuid()}";

            var existing = await _client.GetSecretAsync(name, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            if (existing.HasValue) name = null;
        } while (name == null && ++tries > MaxSecretNameGenerationTries);

        if (name == null) throw new Exception("Could not generate unique key vault secret name.");

        var result = await _client.SetSecretAsync(name, value, cancellationToken).ConfigureAwait(false);

        if (!result.HasValue) throw new Exception("Could not set key vault secret.");

        return name + SecretNameDelimiter + result.Value.Properties.Version;
    }

    public override async Task<string> UnprotectSecretValueAsync(string value,
        CancellationToken cancellationToken = default)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        var parts = value.Split(SecretNameDelimiter);
        if (parts.Length != 3 || parts[0] != SystemId || !Guid.TryParse(parts[1], out var guid))
            throw new InvalidOperationException("Value is no key vault secret from this system.");

        var name = $"{SystemId}{SecretNameDelimiter}{guid}";

        var result = await _client.GetSecretAsync(name, parts[2], cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!result.HasValue) throw new Exception("Could not get key vault secret.");

        return result.Value.Value;
    }
}