using Comfyg.Authentication.Abstractions;
using Comfyg.Contracts.Authentication;
using Comfyg.Contracts.Configuration;
using Comfyg.Core.Abstractions.Configuration;
using Moq;

namespace Comfyg.Tests;

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
    public void Test_LoadConfiguration()
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
            new ConfigurationValue
            {
                Key = "key1",
                Value = "value1"
            },
            new ConfigurationValue
            {
                Key = "key2",
                Value = "value2"
            }
        };

        using var httpClient = _factory.CreateClient();

        var connectionString = $"Endpoint={httpClient.BaseAddress};ClientId={clientId};ClientSecret={clientSecret}";

        _factory.Mock<IClientService>(mock =>
        {
            mock.Setup(cs => cs.GetClientAsync(It.IsAny<string>())).ReturnsAsync(client);
            mock.Setup(cs => cs.ReceiveClientSecretAsync(It.IsAny<IClient>())).ReturnsAsync(clientSecret);
        });

        _factory.Mock<IConfigurationService>(mock =>
        {
            mock.Setup(cs => cs.GetConfigurationValuesAsync(It.IsAny<string>())).ReturnsAsync(configurationValues);
        });

        var configurationBuilder = new ConfigurationBuilder();

        configurationBuilder.AddComfyg(options =>
        {
            // ReSharper disable once AccessToDisposedClosure
            options.Connect(connectionString).OverrideHttpClient(httpClient);
        });

        var configuration = configurationBuilder.Build();

        var configuration1 = configuration["key1"];
        Assert.NotNull(configuration1);
        Assert.Equal("value1", configuration1);

        var configuration2 = configuration["key2"];
        Assert.NotNull(configuration2);
        Assert.Equal("value2", configuration2);
        
        _factory.Mock<IClientService>(mock =>
        {
            mock.Verify(cs => cs.GetClientAsync(It.Is<string>(s => s == clientId)), Times.Once);
            mock.Verify(cs => cs.ReceiveClientSecretAsync(It.Is<IClient>(c => c.ClientId == clientId)), Times.Once);
        });

        _factory.Mock<IConfigurationService>(mock =>
        {
            mock.Verify(cs => cs.GetConfigurationValuesAsync(It.Is<string>(s => s == clientId)), Times.Once);
        });
    }
}