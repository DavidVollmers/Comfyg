namespace Comfyg.Client;

public sealed class ComfygClient : IDisposable
{
    private readonly HttpClient _httpClient = new HttpClient();

    public ComfygClient(string connectionString)
    {
        if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
        
        ParseConnectionString(connectionString);
    }

    private void ParseConnectionString(string connectionString)
    {
        
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}