namespace Comfyg;

/// <summary>
/// Options used to configure the behavior of a Comfyg configuration provider.
/// </summary>
public sealed class ComfygOptions
{
    internal string? ConnectionString { get; private set; }

    internal HttpClient? HttpClient { get; private set; }

    internal IList<string> Tags { get; } = new List<string>();

    /// <summary>
    /// Options to configure the behavior how Comfyg configuration values are provided.
    /// </summary>
    public ComfygValuesOptions Configuration { get; } = new();


    /// <summary>
    /// Options to configure the behavior how Comfyg setting values are provided.
    /// </summary>
    public ComfygValuesOptions Settings { get; } = new(TimeSpan.FromMinutes(5));


    /// <summary>
    /// Options to configure the behavior how Comfyg secret values are provided.
    /// </summary>
    public ComfygValuesOptions Secrets { get; } = new();

    internal ComfygOptions()
    {
    }

    /// <summary>
    /// Connect to a Comfyg store using the provided connection string.
    /// </summary>
    /// <param name="connectionString">The connection string used to connect to the Comfyg store.</param>
    /// <returns><see cref="ComfygOptions"/></returns>
    /// <exception cref="ArgumentNullException"><paramref name="connectionString"/> is null.</exception>
    public ComfygOptions Connect(string connectionString)
    {
        ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        return this;
    }

    /// <summary>
    /// Loads key-value pairs tagged with the provided tags.
    /// </summary>
    /// <param name="tags">The tags to load key-valur pairs for.</param>
    /// <exception cref="ArgumentNullException">An element of <paramref name="tags"/> is null.</exception>
    /// <returns><see cref="ComfygOptions"/></returns>
    public ComfygOptions LoadTags(params string[] tags)
    {
        foreach (var tag in tags)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tags));
            Tags.Add(tag);
        }

        return this;
    }

    internal ComfygOptions OverrideHttpClient(HttpClient httpClient)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        return this;
    }
}
