using Comfyg.Store.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Comfyg.Tests.Common;

public sealed class E2ETestWebApplicationFactory : IDisposable
{
    private readonly IList<HttpClient> _clients = new List<HttpClient>();

    private WebApplication? _webApplication;

    private void EnsureWebApplication()
    {
        if (_webApplication != null) return;

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development
        });

        builder.UseComfygStoreApi();

        _webApplication = builder.Build();

        _webApplication.MapControllers();

        _webApplication.StartAsync();
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
        _webApplication?.StopAsync();
        _webApplication = null;

        foreach (var client in _clients)
        {
            client.Dispose();
        }

        _clients.Clear();
    }
}
