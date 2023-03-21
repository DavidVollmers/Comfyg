using Comfyg.Core.Abstractions.Permissions;
using CoreHelpers.WindowsAzure.Storage.Table;

namespace Comfyg.Core.Permissions;

internal class PermissionService : IPermissionService
{
    private readonly IStorageContext _storageContext;

    public PermissionService(string systemId, IStorageContext storageContext)
    {
        if (systemId == null) throw new ArgumentNullException(nameof(systemId));

        _storageContext = storageContext ?? throw new ArgumentNullException(nameof(storageContext));

        _storageContext.AddAttributeMapper<PermissionEntity>($"{systemId}{nameof(PermissionEntity)}");
        _storageContext.AddAttributeMapper<PermissionEntityMirrored>($"{systemId}{nameof(PermissionEntityMirrored)}");
    }

    public async Task<bool> IsPermittedAsync<T>(string owner, string targetId)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));
        if (targetId == null) throw new ArgumentNullException(nameof(targetId));

        using var context = _storageContext.CreateChildContext();

        var partitionKey = $"{typeof(T).FullName}-{targetId}";
        var ownedValues = (await context.EnableAutoCreateTable().QueryAsync<PermissionEntityMirrored>(partitionKey)
            .ConfigureAwait(false)).ToArray();
        // if no target exists we assume permission (to create it)
        return !ownedValues.Any() || ownedValues.Any(ov => ov.Owner == owner);
    }

    public async Task<IEnumerable<IPermission>> GetPermissionsAsync<T>(string owner)
    {
        using var context = _storageContext.CreateChildContext();
        var partitionKey = $"{owner}-{typeof(T).FullName}";
        return await context.EnableAutoCreateTable().QueryAsync<PermissionEntity>(partitionKey).ConfigureAwait(false);
    }

    public async Task SetPermissionAsync<T>(string owner, string targetId)
    {
        using var context = _storageContext.CreateChildContext();
        context.EnableAutoCreateTable();

        await context.InsertOrReplaceAsync(new PermissionEntity
        {
            Owner = owner,
            TargetId = targetId,
            TargetType = typeof(T).FullName!
        }).ConfigureAwait(false);
        await context.InsertOrReplaceAsync(new PermissionEntityMirrored
        {
            Owner = owner,
            TargetId = targetId,
            TargetType = typeof(T).FullName!
        }).ConfigureAwait(false);
    }
}