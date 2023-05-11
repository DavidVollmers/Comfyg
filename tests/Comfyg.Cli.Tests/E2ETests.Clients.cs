using System.Reflection;
using System.Security.Cryptography;
using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Contracts;
using Comfyg.Tests.Common.Contracts;
using Moq;
using Range = Moq.Range;

namespace Comfyg.Cli.Tests;

public partial class E2ETests
{
    [Fact]
    public async Task Test_SetupClient_AsymmetricKey()
    {
        var clientId = Guid.NewGuid().ToString();
        const string friendlyName = "Asymmetric Test Client";
        var client = new TestClient
        {
            ClientId = clientId, FriendlyName = friendlyName, IsAsymmetric = true
        };
        var keysPath =
            Path.Join(new FileInfo(Assembly.GetAssembly(typeof(E2ETests))!.Location).Directory!.FullName, "test.pem");
        //TODO more accuracy
        const string expectedOutput = $"Successfully created a client for ";

        await ConnectAsSystemAsync();

        _factory.Mock<IClientService>(mock =>
        {
            mock.Setup(cs =>
                    cs.CreateAsymmetricClientAsync(It.IsAny<IClient>(), It.IsAny<RSA>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(client);
        });

        var result = await TestCli.ExecuteAsync($"--nocheck setup client {clientId} \"{friendlyName}\" -k \"{keysPath}\"");

        Assert.Equal(0, result.ExitCode);
        Assert.StartsWith(expectedOutput, result.Output);

        _factory.Mock<IClientService>(mock =>
        {
            mock.Verify(
                cs => cs.CreateAsymmetricClientAsync(It.Is<IClient>(c => c.ClientId == client.ClientId),
                    It.IsAny<RSA>(), It.IsAny<CancellationToken>()), Times.Once);
        });
    }

    [Fact]
    public async Task Test_Connect_AsymmetricKey()
    {
        var clientId = Guid.NewGuid().ToString();
        var assemblyPath = new FileInfo(Assembly.GetAssembly(typeof(E2ETests))!.Location).Directory!.FullName;
        var keysPath = Path.Join(assemblyPath, "test.pem");
        const string friendlyName = "Test Client";
        var client = new TestClient { ClientId = clientId, FriendlyName = friendlyName, IsAsymmetric = true};

        using var rsa = RSA.Create();
        rsa.ImportFromPem(await File.ReadAllTextAsync(keysPath));
        var publicKey = rsa.ExportRSAPublicKey();
        
        using var httpClient = _factory.CreateClient();
        var expectedOutput = @"Successfully connected to " + httpClient.BaseAddress;

        var connectionString = $"Endpoint={httpClient.BaseAddress};ClientId={clientId};ClientSecret={keysPath}";

        _factory.Mock<IClientService>(mock =>
        {
            mock.Setup(cs => cs.GetClientAsync(It.Is<string>(s => s == clientId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(client);
            mock.Setup(cs =>
                    cs.ReceiveClientSecretAsync(It.Is<IClient>(c => c.ClientId == clientId),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(publicKey);
        });

        var result = await TestCli.ExecuteAsync($"--nocheck connect \"{connectionString}\"");

        Assert.Equal(0, result.ExitCode);
        Assert.StartsWith(expectedOutput, result.Output);

        _factory.Mock<IClientService>(mock =>
        {
            mock.Verify(cs => cs.GetClientAsync(It.Is<string>(s => s == clientId), It.IsAny<CancellationToken>()),
                Times.Once);
            mock.Verify(
                cs => cs.ReceiveClientSecretAsync(It.Is<IClient>(c => c.ClientId == clientId),
                    It.IsAny<CancellationToken>()), Times.Once);
        });
    }

    private async Task<IClient> ConnectAsSystemAsync()
    {
        var systemClientId = Guid.NewGuid().ToString();
        var systemClientSecret = Convert.ToBase64String(CreateClientSecret());

        using var httpClient = _factory.CreateClient();
        var expectedOutput = @"Successfully connected to " + httpClient.BaseAddress;

        var connectionString =
            $"Endpoint={httpClient.BaseAddress};ClientId={systemClientId};ClientSecret={systemClientSecret}";

        _factory.Mock<IConfiguration>(mock =>
        {
            mock.Setup(c => c["SystemClientId"]).Returns(systemClientId);
            mock.Setup(c => c["SystemClientSecret"]).Returns(systemClientSecret);
        });

        var result = await TestCli.ExecuteAsync($"--nocheck connect \"{connectionString}\"");

        Assert.Equal(0, result.ExitCode);
        Assert.StartsWith(expectedOutput, result.Output);

        _factory.Mock<IConfiguration>(mock =>
        {
            // if the server is already started => 1 otherwise => 2
            mock.Verify(c => c["SystemClientId"], Times.Between(1, 2, Range.Inclusive));
            mock.Verify(c => c["SystemClientSecret"], Times.Between(1, 2, Range.Inclusive));
        });

        return new TestClient { ClientId = systemClientId, FriendlyName = "Test System Client" };
    }
}
