using Comfyg.Contracts.Configuration;
using Comfyg.Core.Abstractions.Configuration;
using CoreHelpers.WindowsAzure.Storage.Table;

namespace Comfyg.Core.Configuration;

internal class ConfigurationService : IConfigurationService
{
    private readonly IStorageContext _storageContext;

    public ConfigurationService(string systemId, IStorageContext storageContext)
    {
        _storageContext = storageContext;

        _storageContext.AddAttributeMapper<ConfigurationValueEntity>();
        _storageContext.OverrideTableName<ConfigurationValueEntity>($"{nameof(ConfigurationValueEntity)}{systemId}");
        _storageContext.AddAttributeMapper<ConfigurationValueOwnerEntity>();
        _storageContext.OverrideTableName<ConfigurationValueOwnerEntity>(
            $"{nameof(ConfigurationValueOwnerEntity)}{systemId}");
    }

    public async Task AddConfigurationAsync(string owner, string key, string value)
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

        await context.InsertOrReplaceAsync(new ConfigurationValueOwnerEntity
        {
            Key = key,
            Owner = owner
        }).ConfigureAwait(false);
    }

    public async Task<IEnumerable<IConfigurationValue>> GetConfigurationAsync(string owner)
    {
        using var context = _storageContext.CreateChildContext();
        context.EnableAutoCreateTable();

        var values = new List<IConfigurationValue>();

        var ownedValues = await context.QueryAsync<ConfigurationValueOwnerEntity>(owner).ConfigureAwait(false);
        foreach (var ownedValue in ownedValues)
        {
            var latest = await context
                .QueryAsync<ConfigurationValueEntity>(ownedValue.Key, CoreConstants.LatestVersion, 1)
                .ConfigureAwait(false);
            if (latest == null) continue;

            //TODO tags

            values.Add(latest);
        }

        return values;
    }
}