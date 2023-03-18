using Comfyg.Contracts.Changes;
using Comfyg.Contracts.Settings;
using Comfyg.Core.Abstractions.Changes;
using Comfyg.Core.Abstractions.Permissions;
using Comfyg.Core.Abstractions.Settings;
using CoreHelpers.WindowsAzure.Storage.Table;

namespace Comfyg.Core.Settings;

internal class SettingService : ISettingService
{
    private readonly IStorageContext _storageContext;
    private readonly IPermissionService _permissionService;
    private readonly IChangeService _changeService;

    public SettingService(string systemId, IStorageContext storageContext, IPermissionService permissionService,
        IChangeService changeService)
    {
        if (systemId == null) throw new ArgumentNullException(nameof(systemId));

        _storageContext = storageContext ?? throw new ArgumentNullException(nameof(storageContext));
        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
        _changeService = changeService ?? throw new ArgumentNullException(nameof(changeService));

        _storageContext.AddAttributeMapper<SettingValueEntity>();
        _storageContext.OverrideTableName<SettingValueEntity>($"{systemId}{nameof(SettingValueEntity)}");
    }

    public async Task AddSettingValueAsync(string owner, string key, string value)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (value == null) throw new ArgumentNullException(nameof(value));

        using var context = _storageContext.CreateChildContext();
        context.EnableAutoCreateTable();

        foreach (var version in new[] { CoreConstants.LatestVersion, DateTime.UtcNow.Ticks.ToString() })
        {
            await context.InsertOrReplaceAsync(new SettingValueEntity
            {
                Key = key,
                Value = value,
                Version = version
            }).ConfigureAwait(false);

            await _changeService.LogChangeAsync<ISettingValue>(key, ChangeType.Add, owner).ConfigureAwait(false);
        }

        await _permissionService.SetPermissionAsync<ISettingValue>(owner, key).ConfigureAwait(false);
    }

    public async Task<IEnumerable<ISettingValue>> GetSettingValuesAsync(string owner)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));

        using var context = _storageContext.CreateChildContext();
        context.EnableAutoCreateTable();

        var values = new List<ISettingValue>();

        var permissions =
            await _permissionService.GetPermissionsAsync<ISettingValue>(owner).ConfigureAwait(false);
        foreach (var permission in permissions)
        {
            var latest = await context
                .QueryAsync<SettingValueEntity>(permission.TargetId, CoreConstants.LatestVersion, 1)
                .ConfigureAwait(false);

            if (latest == null) continue;

            values.Add(latest);
        }

        return values;
    }

    public async Task<ISettingValue?> GetSettingValueAsync(string key, string version = CoreConstants.LatestVersion)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (version == null) throw new ArgumentNullException(nameof(version));

        using var context = _storageContext.CreateChildContext();

        return await context.EnableAutoCreateTable().QueryAsync<SettingValueEntity>(key, version, 1)
            .ConfigureAwait(false);
    }
}