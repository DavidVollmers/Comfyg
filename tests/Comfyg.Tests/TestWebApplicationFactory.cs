﻿using Comfyg.Authentication.Abstractions;
using Comfyg.Core.Abstractions.Changes;
using Comfyg.Core.Abstractions.Configuration;
using Comfyg.Core.Abstractions.Permissions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Moq;

namespace Comfyg.Tests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly IDictionary<string, Mock> _mocks = new Dictionary<string, Mock>();

    public void ResetMocks()
    {
        foreach (var mock in _mocks)
        {
            mock.Value.Reset();
        }
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<IClientService>(_ => GetMock<IClientService>().Object);
            services.AddSingleton<IConfigurationService>(_ => GetMock<IConfigurationService>().Object);
            services.AddSingleton<IChangeService>(_ => GetMock<IChangeService>().Object);
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