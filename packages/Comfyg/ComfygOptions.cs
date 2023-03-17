using Comfyg.Configuration;

namespace Comfyg;

public sealed class ComfygOptions
{
    internal string? ConnectionString { get; private set; }

    internal HttpClient? HttpClient { get; private set; }

    public ComfygConfigurationOptions Configuration { get; } = new();

    internal ComfygOptions()
    {
    }

    public ComfygOptions Connect(string connectionString)
    {
        ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        return this;
    }

    internal ComfygOptions OverrideHttpClient(HttpClient httpClient)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        return this;
    }
}