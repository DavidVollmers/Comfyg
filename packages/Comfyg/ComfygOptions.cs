namespace Comfyg;

public sealed class ComfygOptions
{
    internal string? ConnectionString { get; private set; }

    internal ComfygOptions()
    {
    }

    public ComfygOptions Connect(string connectionString)
    {
        ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        return this;
    }
}