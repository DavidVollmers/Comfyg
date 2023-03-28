using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Contracts;
using Comfyg.Store.Core.Abstractions;
using Comfyg.Store.Core.Abstractions.Changes;
using Comfyg.Store.Core.Abstractions.Permissions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Comfyg.Tests.Common;

internal class Mocks : IMocks
{
    private readonly IDictionary<string, Mock> _mocks = new Dictionary<string, Mock>();

    public void ResetMocks()
    {
        foreach (var mock in _mocks) mock.Value.Reset();
    }

    public void Mock<T>(Action<Mock<T>> mockProvider) where T : class
    {
        if (mockProvider == null) throw new ArgumentNullException(nameof(mockProvider));
        var mock = GetMock<T>();
        mockProvider(mock);
    }

    public Mock<T> GetMock<T>() where T : class
    {
        var key = typeof(T).FullName!;
        if (_mocks.TryGetValue(key, out var value)) return (Mock<T>)value;

        var mock = new Mock<T>();
        _mocks.Add(key, mock);
        return mock;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IConfiguration>(_ => GetMock<IConfiguration>().Object);

        services.AddSingleton<IClientService>(_ => GetMock<IClientService>().Object);
        services.AddSingleton<IChangeService>(_ => GetMock<IChangeService>().Object);
        services.AddSingleton<IPermissionService>(_ => GetMock<IPermissionService>().Object);
        services.AddSingleton<IValueService<IConfigurationValue>>(_ =>
            GetMock<IValueService<IConfigurationValue>>().Object);
        services.AddSingleton<IValueService<ISettingValue>>(_ => GetMock<IValueService<ISettingValue>>().Object);
        services.AddSingleton<IValueService<ISecretValue>>(_ => GetMock<IValueService<ISecretValue>>().Object);
    }
}
