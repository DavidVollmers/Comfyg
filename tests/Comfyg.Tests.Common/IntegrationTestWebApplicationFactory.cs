using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Comfyg.Tests.Common;

public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly Mocks _mocks = new();

    public void ResetMocks() => _mocks.ResetMocks();

    public void Mock<T>(Action<Mock<T>> mockProvider) where T : class => _mocks.Mock(mockProvider);

    private Mock<T> GetMock<T>() where T : class => _mocks.GetMock<T>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configuration => { configuration.AddJsonFile("appsettings.Test.json"); });

        builder.ConfigureTestServices(_mocks.ConfigureServices);
    }
}
