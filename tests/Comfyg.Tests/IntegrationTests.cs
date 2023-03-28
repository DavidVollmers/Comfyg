using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Contracts;
using Comfyg.Store.Core.Abstractions;
using Comfyg.Store.Core.Abstractions.Changes;
using Comfyg.Tests.Common;
using Comfyg.Tests.Common.Contracts;
using Moq;

namespace Comfyg.Tests;

public class IntegrationTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
    private readonly IntegrationTestWebApplicationFactory _factory;

    public IntegrationTests(IntegrationTestWebApplicationFactory factory)
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
        const string friendlyName = "Test Client";
        var client = new TestClient
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            FriendlyName = friendlyName
        };
        var configurationValues = new[]
        {
            new TestConfigurationValue
            {
                Key = "key1",
                Value = "value1"
            },
            new TestConfigurationValue
            {
                Key = "key2",
                Value = "value2"
            },
            new TestConfigurationValue
            {
                Key = "section1:key1",
                Value = "value3"
            }
        };

        using var httpClient = _factory.CreateClient();

        var connectionString = $"Endpoint={httpClient.BaseAddress};ClientId={clientId};ClientSecret={clientSecret}";

        _factory.Mock<IClientService>(mock =>
        {
            mock.Setup(cs => cs.GetClientAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(client);
            mock.Setup(cs => cs.ReceiveClientSecretAsync(It.IsAny<IClient>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(clientSecret);
        });

        _factory.Mock<IValueService<IConfigurationValue>>(mock =>
        {
            mock.Setup(cs => cs.GetValuesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(configurationValues.ToAsyncEnumerable);
        });

        _factory.Mock<IValueService<ISettingValue>>(mock =>
        {
            mock.Setup(cs => cs.GetValuesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Array.Empty<ISettingValue>().ToAsyncEnumerable);
        });

        _factory.Mock<IValueService<ISecretValue>>(mock =>
        {
            mock.Setup(cs => cs.GetValuesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Array.Empty<ISecretValue>().ToAsyncEnumerable);
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

        var section1 = configuration.GetSection("section1");
        Assert.NotNull(section1);

        var section1Configuration1 = section1["key1"];
        Assert.NotNull(section1Configuration1);
        Assert.Equal("value3", section1Configuration1);

        _factory.Mock<IClientService>(mock =>
        {
            mock.Verify(cs => cs.GetClientAsync(It.Is<string>(s => s == clientId), It.IsAny<CancellationToken>()), Times.Exactly(3));
            mock.Verify(cs => cs.ReceiveClientSecretAsync(It.Is<IClient>(c => c.ClientId == clientId), It.IsAny<CancellationToken>()),
                Times.Exactly(3));
        });

        _factory.Mock<IValueService<IConfigurationValue>>(mock =>
        {
            mock.Verify(cs => cs.GetValuesAsync(It.Is<string>(s => s == clientId), It.IsAny<CancellationToken>()), Times.Once);
        });
    }

    [Fact]
    public void Test_LoadConfiguration_WithChangeDetection()
    {
        var clientId = Guid.NewGuid().ToString();
        var clientSecret = CreateClientSecret();
        const string friendlyName = "Test Client";
        var client = new TestClient
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            FriendlyName = friendlyName
        };
        var configurationValues1 = new[]
        {
            new TestConfigurationValue
            {
                Key = "key1",
                Value = "value1"
            },
            new TestConfigurationValue
            {
                Key = "key2",
                Value = "value2"
            }
        };
        var configurationValue2Change = new TestConfigurationValue
        {
            Key = "key2",
            Value = "newValue2"
        };
        var configurationValue3Change = new TestConfigurationValue
        {
            Key = "key3",
            Value = "value3"
        };
        var changes = new[]
        {
            new ChangeLog
            {
                TargetId = "key2"
            },
            new ChangeLog
            {
                TargetId = "key3"
            }
        };

        using var httpClient = _factory.CreateClient();

        var connectionString = $"Endpoint={httpClient.BaseAddress};ClientId={clientId};ClientSecret={clientSecret}";

        var timer = new TestTimerImplementation();

        _factory.Mock<IClientService>(mock =>
        {
            mock.Setup(cs => cs.GetClientAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(client);
            mock.Setup(cs => cs.ReceiveClientSecretAsync(It.IsAny<IClient>(), It.IsAny<CancellationToken>())).ReturnsAsync(clientSecret);
        });

        _factory.Mock<IValueService<IConfigurationValue>>(mock =>
        {
            mock.Setup(cs => cs.GetValuesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(configurationValues1.ToAsyncEnumerable);
            mock.Setup(cs => cs.GetLatestValueAsync(It.Is<string>(s => s == "key2"), It.IsAny<CancellationToken>()))
                .ReturnsAsync(configurationValue2Change);
            mock.Setup(cs => cs.GetLatestValueAsync(It.Is<string>(s => s == "key3"), It.IsAny<CancellationToken>()))
                .ReturnsAsync(configurationValue3Change);
        });

        _factory.Mock<IValueService<ISettingValue>>(mock =>
        {
            mock.Setup(cs => cs.GetValuesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Array.Empty<ISettingValue>().ToAsyncEnumerable);
        });

        _factory.Mock<IValueService<ISecretValue>>(mock =>
        {
            mock.Setup(cs => cs.GetValuesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Array.Empty<ISecretValue>().ToAsyncEnumerable);
        });

        _factory.Mock<IChangeService>(mock =>
        {
            mock.Setup(cs =>
                    cs.GetChangesForOwnerAsync<IConfigurationValue>(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                .Returns(changes.ToAsyncEnumerable);
            mock.Setup(cs =>
                    cs.GetChangesForOwnerAsync<ISettingValue>(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                .Returns(Array.Empty<IChangeLog>().ToAsyncEnumerable);
            mock.Setup(cs =>
                    cs.GetChangesForOwnerAsync<ISecretValue>(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                .Returns(Array.Empty<IChangeLog>().ToAsyncEnumerable);
        });

        var configurationBuilder = new ConfigurationBuilder();

        configurationBuilder.AddComfyg(options =>
        {
            // ReSharper disable once AccessToDisposedClosure
            options.Connect(connectionString)
                .OverrideHttpClient(httpClient)
                .Configuration.OverrideChangeDetectionTimer(timer);
        });

        var configuration = configurationBuilder.Build();

        var configuration1 = configuration["key1"];
        Assert.NotNull(configuration1);
        Assert.Equal("value1", configuration1);

        var configuration2 = configuration["key2"];
        Assert.NotNull(configuration2);
        Assert.Equal("value2", configuration2);

        var configuration3 = configuration["key3"];
        Assert.Null(configuration3);

        timer.Callback();

        configuration1 = configuration["key1"];
        Assert.NotNull(configuration1);
        Assert.Equal("value1", configuration1);

        configuration2 = configuration["key2"];
        Assert.NotNull(configuration2);
        Assert.Equal("newValue2", configuration2);

        configuration3 = configuration["key3"];
        Assert.NotNull(configuration3);
        Assert.Equal("value3", configuration3);

        _factory.Mock<IClientService>(mock =>
        {
            mock.Verify(cs => cs.GetClientAsync(It.Is<string>(s => s == clientId), It.IsAny<CancellationToken>()), Times.Exactly(5));
            mock.Verify(cs => cs.ReceiveClientSecretAsync(It.Is<IClient>(c => c.ClientId == clientId), It.IsAny<CancellationToken>()),
                Times.Exactly(5));
        });

        _factory.Mock<IValueService<IConfigurationValue>>(mock =>
        {
            mock.Verify(cs => cs.GetValuesAsync(It.Is<string>(s => s == clientId), It.IsAny<CancellationToken>()), Times.Once);
            mock.Verify(cs => cs.GetLatestValueAsync(It.Is<string>(s => s == "key2"), It.IsAny<CancellationToken>()),
                Times.Exactly(2));
            mock.Verify(cs => cs.GetLatestValueAsync(It.Is<string>(s => s == "key3"), It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        });

        _factory.Mock<IChangeService>(mock =>
        {
            mock.Verify(
                cs => cs.GetChangesForOwnerAsync<IConfigurationValue>(It.Is<string>(s => s == clientId),
                    It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        });
    }
}
