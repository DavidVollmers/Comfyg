using System.Reflection;
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
        var publicKeyPath = Path.Join(new FileInfo(Assembly.GetAssembly(typeof(E2ETests))!.Location).Directory!.FullName, "public.pem");
        //TODO more accuracy
        const string expectedOutput = $"Successfully created a client for ";
        
        var systemClient = await ConnectAsSystemAsync();
        
        var result = await TestCli.ExecuteAsync($"setup client {clientId} \"{friendlyName}\" -pk \"{publicKeyPath}\"");
        
        Assert.Equal(0, result.ExitCode);
        Assert.StartsWith(expectedOutput, result.Output);
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

        var result = await TestCli.ExecuteAsync($"connect \"{connectionString}\"");

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
