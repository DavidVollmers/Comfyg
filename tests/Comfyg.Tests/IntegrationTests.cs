﻿using Comfyg.Authentication.Abstractions;
using Comfyg.Contracts.Authentication;
using Comfyg.Contracts.Changes;
using Comfyg.Contracts.Configuration;
using Comfyg.Core.Abstractions;
using Comfyg.Core.Abstractions.Changes;
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
            mock.Setup(cs => cs.GetClientAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(client);
            mock.Setup(cs => cs.ReceiveClientSecretAsync(It.IsAny<IClient>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(clientSecret);
        });

        _factory.Mock<IValueService<IConfigurationValue>>(mock =>
        {
            mock.Setup(cs => cs.GetValuesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(configurationValues.ToAsyncEnumerable);
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
        var friendlyName = "Test Client";
        var client = new Contracts.Authentication.Client
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            FriendlyName = friendlyName
        };
        var configurationValues1 = new[]
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
        var configurationValue2Change = new ConfigurationValue
        {
            Key = "key2",
            Value = "newValue2"
        };
        var configurationValue3Change = new ConfigurationValue
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
            mock.SetupSequence(cs => cs.GetValueAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(configurationValue2Change)
                .ReturnsAsync(configurationValue3Change);
        });

        _factory.Mock<IChangeService>(mock =>
        {
            mock.Setup(cs =>
                    cs.GetChangesForOwnerAsync<IConfigurationValue>(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                .Returns(changes.ToAsyncEnumerable);
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
            mock.Verify(cs => cs.GetValueAsync(It.Is<string>(s => s == "key2"), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
            mock.Verify(cs => cs.GetValueAsync(It.Is<string>(s => s == "key3"), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        });

        _factory.Mock<IChangeService>(mock =>
        {
            mock.Verify(
                cs => cs.GetChangesForOwnerAsync<IConfigurationValue>(It.Is<string>(s => s == clientId),
                    It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        });
    }
}