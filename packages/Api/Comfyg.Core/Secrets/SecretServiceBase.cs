using Comfyg.Contracts.Changes;
using Comfyg.Contracts.Secrets;
using Comfyg.Core.Abstractions.Changes;
using Comfyg.Core.Abstractions.Permissions;
using Comfyg.Core.Abstractions.Secrets;
using CoreHelpers.WindowsAzure.Storage.Table;

namespace Comfyg.Core.Secrets;

public abstract class SecretServiceBase : ISecretService
{
    private readonly IStorageContext? _storageContext;
    private readonly IChangeService? _changeService;
    private readonly IPermissionService? _permissionService;

    protected string SystemId { get; }

    internal SecretServiceBase(string systemId, IStorageContext? storageContext, IChangeService? changeService,
        IPermissionService? permissionService)
    {
        SystemId = systemId ?? throw new ArgumentNullException(nameof(systemId));

        if (storageContext == null) return;

        _storageContext = storageContext ?? throw new ArgumentNullException(nameof(storageContext));
        _changeService = changeService ?? throw new ArgumentNullException(nameof(changeService));
        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));

        _storageContext.AddAttributeMapper<SecretValueEntity>();
        _storageContext.OverrideTableName<SecretValueEntity>($"{systemId}{nameof(SecretValueEntity)}");
    }

    public async Task AddSecretValueAsync(string owner, string key, string value,
        CancellationToken cancellationToken = default)
    {
        if (_storageContext == null) throw new NotImplementedException();
        if (owner == null) throw new ArgumentNullException(nameof(owner));
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (value == null) throw new ArgumentNullException(nameof(value));

        var protectedValue = await ProtectSecretValueAsync(value, cancellationToken).ConfigureAwait(false);

        using var context = _storageContext.CreateChildContext();
        context.EnableAutoCreateTable();

        foreach (var version in new[] {CoreConstants.LatestVersion, DateTime.UtcNow.Ticks.ToString()})
        {
            await context.InsertOrReplaceAsync(new SecretValueEntity
            {
                Key = key,
                Value = protectedValue,
                Version = version
            }).ConfigureAwait(false);

            await _changeService!.LogChangeAsync<ISecretValue>(key, ChangeType.Add, owner).ConfigureAwait(false);
        }

        await _permissionService!.SetPermissionAsync<ISecretValue>(owner, key).ConfigureAwait(false);
    }

    public async Task<IEnumerable<ISecretValue>> GetSecretValuesAsync(string owner,
        CancellationToken cancellationToken = default)
    {
        if (_storageContext == null) throw new NotImplementedException();
        if (owner == null) throw new ArgumentNullException(nameof(owner));

        using var context = _storageContext.CreateChildContext();
        context.EnableAutoCreateTable();

        var values = new List<ISecretValue>();

        var permissions =
            await _permissionService!.GetPermissionsAsync<ISecretValue>(owner).ConfigureAwait(false);
        foreach (var permission in permissions)
        {
            var latest = await context
                .QueryAsync<SecretValueEntity>(permission.TargetId, CoreConstants.LatestVersion, 1)
                .ConfigureAwait(false);

            if (latest == null) continue;

            var unprotectedValue =
                await UnprotectSecretValueAsync(latest.Value, cancellationToken).ConfigureAwait(false);

            values.Add(new SecretValueEntity
            {
                Key = latest.Key,
                Version = latest.Version,
                CreatedAt = latest.CreatedAt,
                Value = unprotectedValue
            });
        }

        return values;
    }

    public async Task<ISecretValue?> GetSecretValueAsync(string key, string version = CoreConstants.LatestVersion,
        CancellationToken cancellationToken = default)
    {
        if (_storageContext == null) throw new NotImplementedException();
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (version == null) throw new ArgumentNullException(nameof(version));

        using var context = _storageContext.CreateChildContext();

        var secret = await context.EnableAutoCreateTable().QueryAsync<SecretValueEntity>(key, version, 1)
            .ConfigureAwait(false);

        if (secret == null) return null;

        var unprotectedValue =
            await UnprotectSecretValueAsync(secret.Value, cancellationToken).ConfigureAwait(false);

        return new SecretValueEntity
        {
            Key = secret.Key,
            Version = secret.Version,
            CreatedAt = secret.CreatedAt,
            Value = unprotectedValue
        };
    }

    public abstract Task<string> ProtectSecretValueAsync(string value, CancellationToken cancellationToken = default);

    public abstract Task<string> UnprotectSecretValueAsync(string value, CancellationToken cancellationToken = default);
}