using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Contracts.Authentication;
using Comfyg.Tests.Common;
using Moq;

namespace Comfyg.Cli.Tests;

public class E2ETests : IClassFixture<E2ETestWebApplicationFactory>
{
    private readonly E2ETestWebApplicationFactory _factory;

    public E2ETests(E2ETestWebApplicationFactory factory)
    {
        _factory = factory;

        // _factory.ResetMocks();
    }

    private static string CreateClientSecret()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }

    [Fact]
    public async Task Test_Connect()
    {
        var clientId = Guid.NewGuid().ToString();
        var clientSecret = CreateClientSecret();
        var friendlyName = "Test Client";
        var client = new Store.Contracts.Authentication.Client
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            FriendlyName = friendlyName
        };

        using var httpClient = _factory.CreateClient();

        // _factory.Mock<IClientService>(mock =>
        // {
        //     mock.Setup(cs => cs.GetClientAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(client);
        //     mock.Setup(cs => cs.ReceiveClientSecretAsync(It.IsAny<IClient>(), It.IsAny<CancellationToken>()))
        //         .ReturnsAsync(clientSecret);
        // });

        var connectionString = $"Endpoint={httpClient.BaseAddress};ClientId={clientId};ClientSecret={clientSecret}";

        var result = await TestCli.ExecuteAsync($"connect \"{connectionString}\"");

        Assert.Equal(0, result.ExitCode);

        // _factory.Mock<IClientService>(mock =>
        // {
        //     mock.Verify(cs => cs.GetClientAsync(It.Is<string>(s => s == clientId), It.IsAny<CancellationToken>()),
        //         Times.Once);
        //     mock.Verify(
        //         cs => cs.ReceiveClientSecretAsync(It.Is<IClient>(c => c.ClientId == clientId),
        //             It.IsAny<CancellationToken>()), Times.Once);
        // });
    }
}
