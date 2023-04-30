using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Contracts;
using Comfyg.Store.Core.Abstractions.Permissions;
using Comfyg.Tests.Common.Contracts;
using Moq;

namespace Comfyg.Client.Tests;

public partial class IntegrationTests
{
    [Fact]
    public async Task Test_SetPermissions_WithFlags()
    {
        var clientId = Guid.NewGuid().ToString();
        var clientSecret = CreateClientSecret();
        const string friendlyName = "Test Client";
        var client = new TestClient { ClientId = clientId, FriendlyName = friendlyName };
        var configurationValueKey1 = Guid.NewGuid().ToString();
        var configurationValuePermissions =
            new IPermission[] { new TestPermission { TargetId = configurationValueKey1 } };
        var secretValueKey1 = Guid.NewGuid().ToString();
        var secretValuePermissions =
            new IPermission[] { new TestPermission { TargetId = secretValueKey1 } };
        var settingValueKey1 = Guid.NewGuid().ToString();
        var settingValuePermissions =
            new IPermission[] { new TestPermission { TargetId = settingValueKey1 } };
        const Permissions expectedPermissions = Permissions.Read | Permissions.Write;
        
        using var httpClient = _factory.CreateClient();

        var connectionString = $"Endpoint={httpClient.BaseAddress};ClientId={clientId};ClientSecret={Convert.ToBase64String(clientSecret)}";
        using var comfygClient = new ComfygClient(connectionString, httpClient);

        _factory.Mock<IClientService>(mock =>
        {
            mock.Setup(cs => cs.GetClientAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(client);
            mock.Setup(cs => cs.ReceiveClientSecretAsync(It.IsAny<IClient>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(clientSecret);
        });

        _factory.Mock<IPermissionService>(mock =>
        {
            mock.Setup(ps =>
                    ps.GetPermissionsAsync<IConfigurationValue>(It.IsAny<string>(), It.IsAny<Permissions>(),
                        It.IsAny<CancellationToken>()))
                .Returns(configurationValuePermissions.ToAsyncEnumerable());
            mock.Setup(ps =>
                    ps.GetPermissionsAsync<ISecretValue>(It.IsAny<string>(), It.IsAny<Permissions>(),
                        It.IsAny<CancellationToken>()))
                .Returns(secretValuePermissions.ToAsyncEnumerable());
            mock.Setup(ps =>
                    ps.GetPermissionsAsync<ISettingValue>(It.IsAny<string>(), It.IsAny<Permissions>(),
                        It.IsAny<CancellationToken>()))
                .Returns(settingValuePermissions.ToAsyncEnumerable());
        });

        await comfygClient.SetPermissionsAsync(clientId, expectedPermissions);

        _factory.Mock<IClientService>(mock =>
        {
            mock.Verify(cs => cs.GetClientAsync(It.Is<string>(s => s == clientId), It.IsAny<CancellationToken>()),
                Times.Exactly(2));
            mock.Verify(
                cs => cs.ReceiveClientSecretAsync(It.Is<IClient>(c => c.ClientId == clientId),
                    It.IsAny<CancellationToken>()), Times.Once);
        });

        _factory.Mock<IPermissionService>(mock =>
        {
            mock.Verify(
                ps => ps.GetPermissionsAsync<IConfigurationValue>(It.Is<string>(s => s == client.ClientId),
                    It.Is<Permissions>(p => p == Permissions.Permit),
                    It.IsAny<CancellationToken>()), Times.Once);
            mock.Verify(
                ps => ps.SetPermissionAsync<IConfigurationValue>(It.Is<string>(s => s == clientId),
                    It.Is<string>(s => s == configurationValueKey1), It.Is<Permissions>(p => p == expectedPermissions),
                    It.IsAny<CancellationToken>()), Times.Once);
            mock.Verify(
                ps => ps.GetPermissionsAsync<ISecretValue>(It.Is<string>(s => s == client.ClientId),
                    It.Is<Permissions>(p => p == Permissions.Permit),
                    It.IsAny<CancellationToken>()), Times.Once);
            mock.Verify(
                ps => ps.SetPermissionAsync<ISecretValue>(It.Is<string>(s => s == clientId),
                    It.Is<string>(s => s == secretValueKey1), It.Is<Permissions>(p => p == expectedPermissions),
                    It.IsAny<CancellationToken>()), Times.Once);
            mock.Verify(
                ps => ps.GetPermissionsAsync<ISettingValue>(It.Is<string>(s => s == client.ClientId),
                    It.Is<Permissions>(p => p == Permissions.Permit),
                    It.IsAny<CancellationToken>()), Times.Once);
            mock.Verify(
                ps => ps.SetPermissionAsync<ISettingValue>(It.Is<string>(s => s == clientId),
                    It.Is<string>(s => s == settingValueKey1), It.Is<Permissions>(p => p == expectedPermissions),
                    It.IsAny<CancellationToken>()), Times.Once);
        });
    }
}
