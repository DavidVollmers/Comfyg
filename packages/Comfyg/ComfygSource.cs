using Comfyg.Client;
using Comfyg.Contracts;
using Microsoft.Extensions.Configuration;

namespace Comfyg;

internal class ComfygSource<T> : IConfigurationSource where T : IComfygValue
{
    private readonly ComfygOptions _options;
    private readonly ComfygValuesOptions _valuesOptions;

    public ComfygSource(ComfygOptions options, ComfygValuesOptions valuesOptions)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _valuesOptions = valuesOptions ?? throw new ArgumentNullException(nameof(valuesOptions));
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        if (_options.ConnectionString == null)
            throw new InvalidOperationException(
                "Please call ComfygOptions.Connect to specify how to connect to the Comfyg API.");
        var client = _options.HttpClient == null
            ? new ComfygClient(_options.ConnectionString)
            : new ComfygClient(_options.ConnectionString, _options.HttpClient);
        return new ComfygProvider<T>(client.Operations<T>(), _valuesOptions.ChangeDetectionTimer);
    }
}