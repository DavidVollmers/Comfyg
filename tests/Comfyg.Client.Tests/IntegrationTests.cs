namespace Comfyg.Client.Tests;

public class IntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public IntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Test_EstablishConnectionAsync()
    {
        var clientId = Guid.NewGuid().ToString();
        var clientSecret = Guid.NewGuid().ToString();
        
        using var httpClient = _factory.CreateClient();
        
        var connectionString = $"Endpoint={httpClient.BaseAddress};ClientId={clientId};ClientSecret={clientSecret}";
        using var client = new ComfygClient(connectionString, httpClient);

        await client.EstablishConnectionAsync();
    }
}