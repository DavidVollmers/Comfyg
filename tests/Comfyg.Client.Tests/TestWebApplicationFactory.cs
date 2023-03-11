using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Moq;

namespace Comfyg.Client.Tests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly IDictionary<string, Mock> _mocks = new Dictionary<string, Mock>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // services.AddSingleton<IClientService>(_ => GetMock<IClientService>().Object);
        });
    }

    public void Mock<T>(Action<Mock<T>> mockDelegate) where T : class
    {
        var mock = GetMock<T>();
        mockDelegate(mock);
    }

    private Mock<T> GetMock<T>() where T : class
    {
        var key = typeof(T).FullName!;
        if (_mocks.TryGetValue(key, out var value)) return (Mock<T>) value;

        var mock = new Mock<T>();
        _mocks.Add(key, mock);
        return mock;
    }
}