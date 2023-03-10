namespace Comfyg.Client;

public sealed class ComfygClient : IDisposable
{
    private readonly HttpClient _httpClient = new();

    public ComfygClient(string connectionString)
    {
        if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

        try
        {
            var connectionInformation =
                connectionString.Split(';').Select(i => i.Split('=')).ToDictionary(i => i[0], i => i[1]);

            _httpClient.BaseAddress = new Uri(connectionInformation["Endpoint"]);
        }
        catch (Exception exception)
        {
            throw new ArgumentException("Invalid connection string", nameof(connectionString), exception);
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}