using Comfyg.Authentication.Abstractions;
using Comfyg.Contracts.Authentication;
using Comfyg.Contracts.Configuration;
using Comfyg.Contracts.Requests;
using Comfyg.Core.Abstractions.Configuration;
using Comfyg.Core.Abstractions.Permissions;
using Moq;

namespace Comfyg.Client.Tests;

public class IntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public IntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;

        _factory.ResetMocks();
    }

    private static string CreateClientSecret()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }

    [Fact]
    public async Task Test_SetupClientAsync()
    {
        var systemClientId = Guid.NewGuid().ToString();
        var systemClientSecret = CreateClientSecret();
        var client = new Contracts.Authentication.Client
        {
            ClientId = Guid.NewGuid().ToString(),
            FriendlyName = "New Client"
        };
        var clientSecret = CreateClientSecret();

        using var httpClient = _factory.CreateClient();

        var connectionString =
            $"Endpoint={httpClient.BaseAddress};ClientId={systemClientId};ClientSecret={systemClientSecret}";
        using var comfygClient = new ComfygClient(connectionString, httpClient);

        _factory.Mock<IConfiguration>(mock =>
        {
            mock.Setup(c => c["ComfygSystemClient"]).Returns(systemClientId);
            mock.Setup(c => c["ComfygSystemClientSecret"]).Returns(systemClientSecret);
        });

        _factory.Mock<IClientService>(mock =>
        {
            mock.Setup(cs => cs.CreateClientAsync(It.IsAny<IClient>())).ReturnsAsync(client);
            mock.Setup(cs => cs.ReceiveClientSecretAsync(It.IsAny<IClient>())).ReturnsAsync(clientSecret);
        });

        var response = await comfygClient.SetupClientAsync(new SetupClientRequest
        {
            Client = client
        });

        Assert.NotNull(response);
        Assert.NotNull(response.Client);
        Assert.Equal(client.ClientId, response.Client.ClientId);
        Assert.Equal(client.FriendlyName, response.Client.FriendlyName);
        Assert.Null(response.Client.ClientSecret);
        Assert.Equal(clientSecret, response.ClientSecret);

        _factory.Mock<IConfiguration>(mock =>
        {
            mock.Verify(c => c["ComfygSystemClient"], Times.AtLeast(2));
            mock.Verify(c => c["ComfygSystemClientSecret"], Times.AtLeast(1));
        });

        _factory.Mock<IClientService>(mock =>
        {
            mock.Verify(cs => cs.GetClientAsync(It.Is<string>(s => s == client.ClientId)), Times.Once);
            mock.Verify(cs => cs.CreateClientAsync(It.Is<IClient>(c => c.ClientId == client.ClientId)), Times.Once);
            mock.Verify(cs => cs.ReceiveClientSecretAsync(It.Is<IClient>(c => c.ClientId == client.ClientId)),
                Times.Once);
        });
    }

    [Fact]
    public async Task Test_EstablishConnectionAsync()
    {
        var clientId = Guid.NewGuid().ToString();
        var clientSecret = CreateClientSecret();
        var friendlyName = "Test Client";
        var client = new Contracts.Authentication.Client
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            FriendlyName = friendlyName
        };

        using var httpClient = _factory.CreateClient();

        var connectionString = $"Endpoint={httpClient.BaseAddress};ClientId={clientId};ClientSecret={clientSecret}";
        using var comfygClient = new ComfygClient(connectionString, httpClient);

        _factory.Mock<IClientService>(mock =>
        {
            mock.Setup(cs => cs.GetClientAsync(It.IsAny<string>())).ReturnsAsync(client);
            mock.Setup(cs => cs.ReceiveClientSecretAsync(It.IsAny<IClient>())).ReturnsAsync(clientSecret);
        });

        var response = await comfygClient.EstablishConnectionAsync();

        Assert.NotNull(response);
        Assert.NotNull(response.Client);
        Assert.Equal(clientId, response.Client.ClientId);
        Assert.Equal(friendlyName, response.Client.FriendlyName);
        Assert.Null(response.Client.ClientSecret);

        _factory.Mock<IClientService>(mock =>
        {
            mock.Verify(cs => cs.GetClientAsync(It.Is<string>(s => s == clientId)), Times.Once);
            mock.Verify(cs => cs.ReceiveClientSecretAsync(It.Is<IClient>(c => c.ClientId == clientId)), Times.Once);
        });
    }

    [Fact]
    public async Task Test_AddConfigurationAsync()
    {
        var clientId = Guid.NewGuid().ToString();
        var clientSecret = CreateClientSecret();
        var friendlyName = "Test Client";
        var client = new Contracts.Authentication.Client
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            FriendlyName = friendlyName
        };
        var configurationValues = new[]
        {
            new ConfigurationValue("key1", "value1"),
            new ConfigurationValue("key2", "value2")
        };

        using var httpClient = _factory.CreateClient();

        var connectionString = $"Endpoint={httpClient.BaseAddress};ClientId={clientId};ClientSecret={clientSecret}";
        using var comfygClient = new ComfygClient(connectionString, httpClient);

        _factory.Mock<IClientService>(mock =>
        {
            mock.Setup(cs => cs.GetClientAsync(It.IsAny<string>())).ReturnsAsync(client);
            mock.Setup(cs => cs.ReceiveClientSecretAsync(It.IsAny<IClient>())).ReturnsAsync(clientSecret);
        });

        _factory.Mock<IPermissionService>(mock =>
        {
            mock.Setup(ps => ps.IsPermittedAsync<IConfigurationValue>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);
        });

        await comfygClient.AddConfigurationAsync(new AddConfigurationValuesRequest
        {
            ConfigurationValues = configurationValues
        });

        _factory.Mock<IClientService>(mock =>
        {
            mock.Verify(cs => cs.GetClientAsync(It.Is<string>(s => s == clientId)), Times.Once);
            mock.Verify(cs => cs.ReceiveClientSecretAsync(It.Is<IClient>(c => c.ClientId == clientId)), Times.Once);
        });

        _factory.Mock<IPermissionService>(mock =>
        {
            mock.Verify(
                ps => ps.IsPermittedAsync<IConfigurationValue>(It.Is<string>(s => s == clientId),
                    It.Is<string>(s => s == "key1")), Times.Once);
            mock.Verify(
                ps => ps.IsPermittedAsync<IConfigurationValue>(It.Is<string>(s => s == clientId),
                    It.Is<string>(s => s == "key2")), Times.Once);
        });

        _factory.Mock<IConfigurationService>(mock =>
        {
            mock.Verify(cs => cs.AddConfigurationValueAsync(It.Is<string>(s => s == clientId),
                It.Is<string>(s => s == "key1"), It.Is<string>(s => s == "value1")), Times.Once);
            mock.Verify(cs => cs.AddConfigurationValueAsync(It.Is<string>(s => s == clientId),
                It.Is<string>(s => s == "key2"), It.Is<string>(s => s == "value2")), Times.Once);
        });
    }
}