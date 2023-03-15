using Comfyg.Contracts.Changes;
using Comfyg.Contracts.Configuration;
using Comfyg.Core.Abstractions.Changes;
using Comfyg.Core.Abstractions.Configuration;
using Comfyg.Core.Abstractions.Permissions;
using CoreHelpers.WindowsAzure.Storage.Table;

namespace Comfyg.Core.Configuration;

internal class ConfigurationService : IConfigurationService
{
    private readonly IStorageContext _storageContext;
    private readonly IPermissionService _permissionService;
    private readonly IChangeService _changeService;

    public ConfigurationService(string systemId, IStorageContext storageContext, IPermissionService permissionService,
        IChangeService changeService)
    {
        if (systemId == null) throw new ArgumentNullException(nameof(systemId));
        
        _storageContext = storageContext ?? throw new ArgumentNullException(nameof(storageContext));
        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
        _changeService = changeService ?? throw new ArgumentNullException(nameof(changeService));

        _storageContext.AddAttributeMapper<ConfigurationValueEntity>();
        _storageContext.OverrideTableName<ConfigurationValueEntity>($"{systemId}{nameof(ConfigurationValueEntity)}");
    }

    public async Task AddConfigurationValueAsync(string owner, string key, string value)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (value == null) throw new ArgumentNullException(nameof(value));
        
        using var context = _storageContext.CreateChildContext();
        context.EnableAutoCreateTable();

        foreach (var version in new[] {CoreConstants.LatestVersion, DateTime.UtcNow.Ticks.ToString()})
        {
            await context.InsertOrReplaceAsync(new ConfigurationValueEntity
            {
                Key = key,
                Value = value,
                Version = version
            }).ConfigureAwait(false);

            await _changeService.LogChangeAsync<IConfigurationValue>(key, ChangeType.Add, owner).ConfigureAwait(false);
        }

        await _permissionService.SetPermissionAsync<IConfigurationValue>(owner, key).ConfigureAwait(false);
    }

    public async Task<IEnumerable<IConfigurationValue>> GetConfigurationValuesAsync(string owner)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));
        
        using var context = _storageContext.CreateChildContext();
        context.EnableAutoCreateTable();

        var values = new List<IConfigurationValue>();

        var permissions =
            await _permissionService.GetPermissionsAsync<IConfigurationValue>(owner).ConfigureAwait(false);
        foreach (var permission in permissions)
        {
            var latest = await context
                .QueryAsync<ConfigurationValueEntity>(permission.TargetId, CoreConstants.LatestVersion, 1)
                .ConfigureAwait(false);

            if (latest == null) continue;

            values.Add(latest);
        }

        return values;
    }

    public async Task<IConfigurationValue?> GetConfigurationValueAsync(string key,
        string version = CoreConstants.LatestVersion)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (version == null) throw new ArgumentNullException(nameof(version));
        
        using var context = _storageContext.CreateChildContext();

        return await context.EnableAutoCreateTable().QueryAsync<ConfigurationValueEntity>(key, version, 1)
            .ConfigureAwait(false);
    }
}