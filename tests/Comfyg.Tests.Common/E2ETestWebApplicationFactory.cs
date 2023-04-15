using Comfyg.Store.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Comfyg.Tests.Common;

public sealed class E2ETestWebApplicationFactory<TEntryPoint> : IDisposable where TEntryPoint : class
{
    private readonly IList<HttpClient> _clients = new List<HttpClient>();
    private readonly Mocks _mocks = new();

    private WebApplication? _webApplication;

    public ITestOutputHelper? TestOutputHelper { get; set; }

    public void ResetMocks() => _mocks.ResetMocks();

    public void Mock<T>(Action<Mock<T>> mockProvider) where T : class => _mocks.Mock(mockProvider);

    private Mock<T> GetMock<T>() where T : class => _mocks.GetMock<T>();

    private void EnsureWebApplication()
    {
        if (_webApplication != null) return;

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development
        });

        var mvcBuilder = builder.Services.AddControllers(options =>
        {
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
        });
        mvcBuilder.PartManager.ApplicationParts.Add(new AssemblyPart(typeof(TEntryPoint).Assembly));

        builder.UseComfygStoreApi();

        if (TestOutputHelper != null) builder.Logging.AddProvider(new TestLoggerProvider(TestOutputHelper));

        builder.Configuration.AddJsonFile("appsettings.Test.json");

        _mocks.ConfigureServices(builder.Services);

        builder.Services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.All;
        });
        
        _webApplication = builder.Build();

        _webApplication.MapControllers();

        _webApplication.UseAuthentication();
        _webApplication.UseAuthorization();

        _webApplication.UseHttpLogging();

        _webApplication.StartAsync().GetAwaiter().GetResult();
    }

    public HttpClient CreateClient()
    {
        EnsureWebApplication();

        var client = new HttpClient();
        client.BaseAddress = new Uri(_webApplication!.Urls.First());

        _clients.Add(client);

        return client;
    }

    public void Dispose()
    {
        _webApplication?.StopAsync().GetAwaiter().GetResult();
        _webApplication = null;

        foreach (var client in _clients)
        {
            client.Dispose();
        }

        _clients.Clear();
    }
}
