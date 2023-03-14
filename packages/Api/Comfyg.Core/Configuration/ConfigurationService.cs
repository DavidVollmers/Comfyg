using Comfyg.Contracts.Configuration;
using Comfyg.Core.Abstractions.Configuration;
using Comfyg.Core.Abstractions.Permissions;
using CoreHelpers.WindowsAzure.Storage.Table;

namespace Comfyg.Core.Configuration;

internal class ConfigurationService : IConfigurationService
{
    private readonly IStorageContext _storageContext;
    private readonly IPermissionService _permissionService;

    public ConfigurationService(string systemId, IStorageContext storageContext, IPermissionService permissionService)
    {
        _storageContext = storageContext;
        _permissionService = permissionService;

        _storageContext.AddAttributeMapper<ConfigurationValueEntity>();
        _storageContext.OverrideTableName<ConfigurationValueEntity>($"{nameof(ConfigurationValueEntity)}{systemId}");
    }

    public async Task AddConfigurationValueAsync(string owner, string key, string value)
    {
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
        }

        await _permissionService.SetPermissionAsync<IConfigurationValue>(owner, key).ConfigureAwait(false);
    }

    public async Task<IEnumerable<IConfigurationValue>> GetConfigurationValuesAsync(string owner)
    {
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

            //TODO tags

            values.Add(latest);
        }

        return values;
    }
}