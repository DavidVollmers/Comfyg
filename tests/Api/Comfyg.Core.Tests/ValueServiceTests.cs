using Comfyg.Contracts.Configuration;
using Comfyg.Contracts.Secrets;
using Comfyg.Contracts.Settings;
using Comfyg.Core.Abstractions;
using Comfyg.Core.Abstractions.Changes;
using Comfyg.Core.Abstractions.Permissions;
using Comfyg.Core.Configuration;
using Comfyg.Core.Secrets;
using Comfyg.Core.Settings;
using CoreHelpers.WindowsAzure.Storage.Table;
using Moq;

namespace Comfyg.Core.Tests;

public class ValueServiceTests
{
    [Fact]
    public void Test_StorageTableName_ConfigurationValue()
    {
        var systemId = Guid.NewGuid().ToString();
        var expectedTableName = $"{systemId}{nameof(ConfigurationValueEntity)}";

        var storageContext = new Mock<IStorageContext>();

        var permissionService = new Mock<IPermissionService>();

        var changeService = new Mock<IChangeService>();

        IValueService<IConfigurationValue> valueService =
            new ValueService<IConfigurationValue, ConfigurationValueEntity>(systemId, storageContext.Object,
                permissionService.Object, changeService.Object);

        storageContext.Verify(
            sc => sc.AddAttributeMapper<ConfigurationValueEntity>(It.Is<string>(s => s == expectedTableName)),
            Times.Once);
    }
    
    [Fact]
    public void Test_StorageTableName_SettingValue()
    {
        var systemId = Guid.NewGuid().ToString();
        var expectedTableName = $"{systemId}{nameof(SettingValueEntity)}";

        var storageContext = new Mock<IStorageContext>();

        var permissionService = new Mock<IPermissionService>();

        var changeService = new Mock<IChangeService>();

        IValueService<ISettingValue> valueService =
            new ValueService<ISettingValue, SettingValueEntity>(systemId, storageContext.Object,
                permissionService.Object, changeService.Object);

        storageContext.Verify(
            sc => sc.AddAttributeMapper<SettingValueEntity>(It.Is<string>(s => s == expectedTableName)),
            Times.Once);
    }
    
    [Fact]
    public void Test_StorageTableName_SecretValue()
    {
        var systemId = Guid.NewGuid().ToString();
        var expectedTableName = $"{systemId}{nameof(SecretValueEntity)}";

        var storageContext = new Mock<IStorageContext>();

        var permissionService = new Mock<IPermissionService>();

        var changeService = new Mock<IChangeService>();

        IValueService<ISecretValue> valueService =
            new ValueService<ISecretValue, SecretValueEntity>(systemId, storageContext.Object,
                permissionService.Object, changeService.Object);

        storageContext.Verify(
            sc => sc.AddAttributeMapper<SecretValueEntity>(It.Is<string>(s => s == expectedTableName)),
            Times.Once);
    }
}