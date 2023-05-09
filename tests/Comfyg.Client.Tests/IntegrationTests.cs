using System.Security.Cryptography;
using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Contracts;
using Comfyg.Store.Core.Abstractions;
using Comfyg.Store.Core.Abstractions.Permissions;
using Comfyg.Tests.Common;
using Comfyg.Tests.Common.Contracts;
using Moq;
using Range = Moq.Range;

namespace Comfyg.Client.Tests;

public partial class IntegrationTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
    private readonly IntegrationTestWebApplicationFactory _factory;

    public IntegrationTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;

        _factory.ResetMocks();
    }

    private static byte[] CreateClientSecret()
    {
        return Guid.NewGuid().ToByteArray();
    }

    [Fact]
    public async Task Test_SetupClientAsync()
    {
        var systemClientId = Guid.NewGuid().ToString();
        var systemClientSecret = Convert.ToBase64String(CreateClientSecret());
        var client = new TestClient { ClientId = Guid.NewGuid().ToString(), FriendlyName = "New Client" };
        var clientSecret = CreateClientSecret();

        using var httpClient = _factory.CreateClient();

        var connectionString =
            $"Endpoint={httpClient.BaseAddress};ClientId={systemClientId};ClientSecret={systemClientSecret}";
        using var comfygClient = new ComfygClient(connectionString, httpClient);

        _factory.Mock<IConfiguration>(mock =>
        {
            mock.Setup(c => c["SystemClientId"]).Returns(systemClientId);
            mock.Setup(c => c["SystemClientSecret"]).Returns(systemClientSecret);
        });

        _factory.Mock<IClientService>(mock =>
        {
            mock.Setup(cs => cs.CreateSymmetricClientAsync(It.IsAny<IClient>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(client);
            mock.Setup(cs => cs.ReceiveClientSecretAsync(It.IsAny<IClient>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(clientSecret);
        });

        var response = await comfygClient.SetupClientAsync(client);

        Assert.NotNull(response);
        Assert.NotNull(response.Client);
        Assert.Equal(client.ClientId, response.Client.ClientId);
        Assert.Equal(client.FriendlyName, response.Client.FriendlyName);
        Assert.Null(response.Client.ClientSecret);
        Assert.Equal(Convert.ToBase64String(clientSecret), response.ClientSecret);

        _factory.Mock<IConfiguration>(mock =>
        {
            // if the server is already started => 1 otherwise => 2
            mock.Verify(c => c["SystemClientId"], Times.Between(1, 2, Range.Inclusive));
            mock.Verify(c => c["SystemClientSecret"], Times.Between(1, 2, Range.Inclusive));
        });

        _factory.Mock<IClientService>(mock =>
        {
            mock.Verify(
                cs => cs.GetClientAsync(It.Is<string>(s => s == client.ClientId), It.IsAny<CancellationToken>()),
                Times.Once);
            mock.Verify(
                cs => cs.CreateSymmetricClientAsync(It.Is<IClient>(c => c.ClientId == client.ClientId),
                    It.IsAny<CancellationToken>()), Times.Once);
            mock.Verify(
                cs => cs.ReceiveClientSecretAsync(It.Is<IClient>(c => c.ClientId == client.ClientId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        });
    }

    [Fact]
    public async Task Test_EstablishConnectionAsync()
    {
        var clientId = Guid.NewGuid().ToString();
        var clientSecret = CreateClientSecret();
        const string friendlyName = "Test Client";
        var client = new TestClient { ClientId = clientId, FriendlyName = friendlyName };

        using var httpClient = _factory.CreateClient();

        var connectionString =
            $"Endpoint={httpClient.BaseAddress};ClientId={clientId};ClientSecret={Convert.ToBase64String(clientSecret)}";
        using var comfygClient = new ComfygClient(connectionString, httpClient);

        _factory.Mock<IClientService>(mock =>
        {
            mock.Setup(cs => cs.GetClientAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(client);
            mock.Setup(cs => cs.ReceiveClientSecretAsync(It.IsAny<IClient>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(clientSecret);
        });

        var response = await comfygClient.EstablishConnectionAsync();

        Assert.NotNull(response);
        Assert.NotNull(response.Client);
        Assert.Equal(clientId, response.Client.ClientId);
        Assert.Equal(friendlyName, response.Client.FriendlyName);
        Assert.Null(response.Client.ClientSecret);

        _factory.Mock<IClientService>(mock =>
        {
            mock.Verify(cs => cs.GetClientAsync(It.Is<string>(s => s == clientId), It.IsAny<CancellationToken>()),
                Times.Once);
            mock.Verify(
                cs => cs.ReceiveClientSecretAsync(It.Is<IClient>(c => c.ClientId == clientId),
                    It.IsAny<CancellationToken>()), Times.Once);
        });
    }

    [Fact]
    public async Task Test_EstablishConnectionAsync_WithEnvironmentVariables()
    {
        var clientId = Guid.NewGuid().ToString();
        var clientSecret = CreateClientSecret();
        const string friendlyName = "Test Client";
        var client = new TestClient { ClientId = clientId, FriendlyName = friendlyName };
        var envVar1 = Guid.NewGuid().ToString("N");
        Environment.SetEnvironmentVariable(envVar1, clientId);

        using var httpClient = _factory.CreateClient();

        var connectionString =
            $"Endpoint={httpClient.BaseAddress};ClientId=${envVar1};ClientSecret={Convert.ToBase64String(clientSecret)}";
        using var comfygClient = new ComfygClient(connectionString, httpClient);

        _factory.Mock<IClientService>(mock =>
        {
            mock.Setup(cs => cs.GetClientAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(client);
            mock.Setup(cs => cs.ReceiveClientSecretAsync(It.IsAny<IClient>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(clientSecret);
        });

        var response = await comfygClient.EstablishConnectionAsync();

        Assert.NotNull(response);
        Assert.NotNull(response.Client);
        Assert.Equal(clientId, response.Client.ClientId);
        Assert.Equal(friendlyName, response.Client.FriendlyName);
        Assert.Null(response.Client.ClientSecret);

        _factory.Mock<IClientService>(mock =>
        {
            mock.Verify(cs => cs.GetClientAsync(It.Is<string>(s => s == clientId), It.IsAny<CancellationToken>()),
                Times.Once);
            mock.Verify(
                cs => cs.ReceiveClientSecretAsync(It.Is<IClient>(c => c.ClientId == clientId),
                    It.IsAny<CancellationToken>()), Times.Once);
        });
        
        Environment.SetEnvironmentVariable(envVar1, null);
    }

    [Fact]
    public async Task Test_AddConfigurationAsync()
    {
        var clientId = Guid.NewGuid().ToString();
        var clientSecret = CreateClientSecret();
        const string friendlyName = "Test Client";
        var client = new TestClient { ClientId = clientId, FriendlyName = friendlyName };
        var configurationValues = new[]
        {
            new ConfigurationValue("key1", "value1"), new ConfigurationValue("key2", "value2")
        };
        var expectedHash1 = Convert.ToBase64String(SHA256.HashData("value1"u8.ToArray()));
        var expectedHash2 = Convert.ToBase64String(SHA256.HashData("value2"u8.ToArray()));

        using var httpClient = _factory.CreateClient();

        var connectionString =
            $"Endpoint={httpClient.BaseAddress};ClientId={clientId};ClientSecret={Convert.ToBase64String(clientSecret)}";
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
                    ps.IsPermittedAsync<IConfigurationValue>(It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<Permissions>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
        });

        await comfygClient.Configuration.AddValuesAsync(configurationValues);

        _factory.Mock<IClientService>(mock =>
        {
            mock.Verify(cs => cs.GetClientAsync(It.Is<string>(s => s == clientId), It.IsAny<CancellationToken>()),
                Times.Once);
            mock.Verify(
                cs => cs.ReceiveClientSecretAsync(It.Is<IClient>(c => c.ClientId == clientId),
                    It.IsAny<CancellationToken>()), Times.Once);
        });

        _factory.Mock<IPermissionService>(mock =>
        {
            mock.Verify(
                ps => ps.IsPermittedAsync<IConfigurationValue>(It.Is<string>(s => s == clientId),
                    It.Is<string>(s => s == "key1"), It.Is<Permissions>(p => p == Permissions.Write),
                    It.Is<bool>(b => b), It.IsAny<CancellationToken>()), Times.Once);
            mock.Verify(
                ps => ps.IsPermittedAsync<IConfigurationValue>(It.Is<string>(s => s == clientId),
                    It.Is<string>(s => s == "key2"), It.Is<Permissions>(p => p == Permissions.Write),
                    It.Is<bool>(b => b), It.IsAny<CancellationToken>()), Times.Once);
        });

        _factory.Mock<IValueService<IConfigurationValue>>(mock =>
        {
            mock.Verify(cs => cs.AddValueAsync(It.Is<string>(s => s == clientId),
                    It.Is<string>(s => s == "key1"), It.Is<string>(s => s == "value1"),
                    It.Is<bool>(b => !b), It.Is<string>(s => s == expectedHash1), It.IsAny<CancellationToken>()),
                Times.Once);
            mock.Verify(cs => cs.AddValueAsync(It.Is<string>(s => s == clientId),
                    It.Is<string>(s => s == "key2"), It.Is<string>(s => s == "value2"),
                    It.Is<bool>(b => !b), It.Is<string>(s => s == expectedHash2), It.IsAny<CancellationToken>()),
                Times.Once);
        });
    }
}
