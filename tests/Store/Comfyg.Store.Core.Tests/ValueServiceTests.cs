using Azure.Data.Tables;
using Comfyg.Store.Contracts;
using Comfyg.Store.Core.Abstractions;
using Comfyg.Store.Core.Abstractions.Changes;
using Comfyg.Store.Core.Abstractions.Permissions;
using Comfyg.Store.Core.Configuration;
using Comfyg.Store.Core.Secrets;
using Comfyg.Store.Core.Settings;
using Moq;

namespace Comfyg.Store.Core.Tests;

public class ValueServiceTests
{
    [Fact]
    public void Test_StorageTableName_ConfigurationValue()
    {
        const string systemId = "system";
        const string expectedTableName = $"{systemId}{nameof(ConfigurationValueEntity)}";

        var tableServiceClient = new Mock<TableServiceClient>();

        var permissionService = new Mock<IPermissionService>();

        var changeService = new Mock<IChangeService>();

        IValueService<IConfigurationValue> valueService =
            new ValueService<IConfigurationValue, ConfigurationValueEntity>(systemId, tableServiceClient.Object,
                permissionService.Object, changeService.Object);

        tableServiceClient.Verify(sc => sc.GetTableClient(It.Is<string>(s => s == expectedTableName)), Times.Once);
    }

    [Fact]
    public void Test_StorageTableName_SettingValue()
    {
        const string systemId = "system";
        const string expectedTableName = $"{systemId}{nameof(SettingValueEntity)}";

        var tableServiceClient = new Mock<TableServiceClient>();

        var permissionService = new Mock<IPermissionService>();

        var changeService = new Mock<IChangeService>();

        IValueService<ISettingValue> valueService =
            new ValueService<ISettingValue, SettingValueEntity>(systemId, tableServiceClient.Object,
                permissionService.Object, changeService.Object);

        tableServiceClient.Verify(sc => sc.GetTableClient(It.Is<string>(s => s == expectedTableName)), Times.Once);
    }

    [Fact]
    public void Test_StorageTableName_SecretValue()
    {
        const string systemId = "system";
        const string expectedTableName = $"{systemId}{nameof(SecretValueEntity)}";

        var tableServiceClient = new Mock<TableServiceClient>();

        var permissionService = new Mock<IPermissionService>();

        var changeService = new Mock<IChangeService>();

        IValueService<ISecretValue> valueService =
            new ValueService<ISecretValue, SecretValueEntity>(systemId, tableServiceClient.Object,
                permissionService.Object, changeService.Object);

        tableServiceClient.Verify(sc => sc.GetTableClient(It.Is<string>(s => s == expectedTableName)), Times.Once);
    }
}
