using Comfyg.Contracts;
using Comfyg.Contracts.Changes;
using Comfyg.Core.Abstractions;
using Comfyg.Core.Abstractions.Changes;
using Comfyg.Core.Abstractions.Permissions;
using CoreHelpers.WindowsAzure.Storage.Table;

namespace Comfyg.Core;

internal class ValueService<TValue, TEntity> : IValueService<TValue>
    where TValue : IComfygValue
    where TEntity : class, TValue, ISerializableComfygValue, new()
{
    private readonly IStorageContext _storageContext;
    private readonly IPermissionService _permissionService;
    private readonly IChangeService _changeService;

    public ValueService(string systemId, IStorageContext storageContext, IPermissionService permissionService,
        IChangeService changeService)
    {
        if (systemId == null) throw new ArgumentNullException(nameof(systemId));

        _storageContext = storageContext ?? throw new ArgumentNullException(nameof(storageContext));
        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
        _changeService = changeService ?? throw new ArgumentNullException(nameof(changeService));

        _storageContext.AddAttributeMapper<TEntity>($"{systemId}{typeof(TEntity).Name}");
    }

    public async Task AddValueAsync(string owner, string key, string value)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (value == null) throw new ArgumentNullException(nameof(value));

        using var context = _storageContext.CreateChildContext();
        context.EnableAutoCreateTable();

        foreach (var version in new[]
                     { CoreConstants.LatestVersion, (long.MaxValue - DateTime.UtcNow.Ticks).ToString() })
        {
            await context.InsertOrReplaceAsync(new TEntity
            {
                Key = key,
                Value = value,
                Version = version
            }).ConfigureAwait(false);

            await _changeService.LogChangeAsync<TValue>(key, ChangeType.Add, owner).ConfigureAwait(false);
        }

        await _permissionService.SetPermissionAsync<TValue>(owner, key).ConfigureAwait(false);
    }

    public async Task<IEnumerable<TValue>> GetValuesAsync(string owner)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));

        using var context = _storageContext.CreateChildContext();
        context.EnableAutoCreateTable();

        var values = new List<TValue>();

        var permissions = await _permissionService.GetPermissionsAsync<TValue>(owner).ConfigureAwait(false);
        foreach (var permission in permissions)
        {
            var latest = await context
                .QueryAsync<TEntity>(permission.TargetId, CoreConstants.LatestVersion, 1)
                .ConfigureAwait(false);

            if (latest == null) continue;

            values.Add(latest);
        }

        return values;
    }

    public async Task<TValue?> GetValueAsync(string key, string version = CoreConstants.LatestVersion)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (version == null) throw new ArgumentNullException(nameof(version));

        using var context = _storageContext.CreateChildContext();

        return await context.EnableAutoCreateTable().QueryAsync<TEntity>(key, version, 1)
            .ConfigureAwait(false);
    }
}