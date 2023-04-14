using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Contracts;
using Comfyg.Store.Core.Abstractions.Permissions;
using Comfyg.Tests.Common.Contracts;
using Moq;

namespace Comfyg.Cli.Tests;

public partial class E2ETests
{
    [Fact]
    public async Task Test_SetPermissions()
    {
        var targetClientId = Guid.NewGuid().ToString();
        var expectedOutput = $"Successfully set permissions for \"{targetClientId}\"";
        var targetClient = new TestClient { ClientId = targetClientId };
        var configurationValueKey1 = Guid.NewGuid().ToString();
        var configurationValuePermissions =
            new IPermission[] { new TestPermission { TargetId = configurationValueKey1 } };
        var secretValueKey1 = Guid.NewGuid().ToString();
        var secretValuePermissions =
            new IPermission[] { new TestPermission { TargetId = secretValueKey1 } };
        var settingValueKey1 = Guid.NewGuid().ToString();
        var settingValuePermissions =
            new IPermission[] { new TestPermission { TargetId = settingValueKey1 } };

        _factory.Mock<IClientService>(mock =>
        {
            mock.Setup(cs => cs.GetClientAsync(It.Is<string>(s => s == targetClientId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(targetClient);
        });

        _factory.Mock<IPermissionService>(mock =>
        {
            mock.Setup(ps =>
                    ps.GetPermissionsAsync<IConfigurationValue>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(configurationValuePermissions.ToAsyncEnumerable());
            mock.Setup(ps =>
                    ps.GetPermissionsAsync<ISecretValue>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(secretValuePermissions.ToAsyncEnumerable());
            mock.Setup(ps =>
                    ps.GetPermissionsAsync<ISettingValue>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(settingValuePermissions.ToAsyncEnumerable());
        });

        var client = await ConnectAsync();

        var result = await TestCli.ExecuteAsync($"set permissions {targetClientId}");

        Assert.Equal(0, result.ExitCode);
        Assert.StartsWith(expectedOutput, result.Output);

        _factory.Mock<IClientService>(mock =>
        {
            mock.Verify(cs => cs.GetClientAsync(It.Is<string>(s => s == targetClientId), It.IsAny<CancellationToken>()),
                Times.Once);
        });

        _factory.Mock<IPermissionService>(mock =>
        {
            mock.Verify(
                ps => ps.GetPermissionsAsync<IConfigurationValue>(It.Is<string>(s => s == client.ClientId),
                    It.IsAny<CancellationToken>()), Times.Once);
            mock.Verify(
                ps => ps.SetPermissionAsync<IConfigurationValue>(It.Is<string>(s => s == targetClientId),
                    It.Is<string>(s => s == configurationValueKey1), It.IsAny<CancellationToken>()), Times.Once);
            mock.Verify(
                ps => ps.GetPermissionsAsync<ISecretValue>(It.Is<string>(s => s == client.ClientId),
                    It.IsAny<CancellationToken>()), Times.Once);
            mock.Verify(
                ps => ps.SetPermissionAsync<ISecretValue>(It.Is<string>(s => s == targetClientId),
                    It.Is<string>(s => s == secretValueKey1), It.IsAny<CancellationToken>()), Times.Once);
            mock.Verify(
                ps => ps.GetPermissionsAsync<ISettingValue>(It.Is<string>(s => s == client.ClientId),
                    It.IsAny<CancellationToken>()), Times.Once);
            mock.Verify(
                ps => ps.SetPermissionAsync<ISettingValue>(It.Is<string>(s => s == targetClientId),
                    It.Is<string>(s => s == settingValueKey1), It.IsAny<CancellationToken>()), Times.Once);
        });
    }
}
