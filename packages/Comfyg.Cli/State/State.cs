using System.Text.Json;

namespace Comfyg.Cli.State;

internal class State
{
    public static State User = new(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

    private readonly DirectoryInfo _directory;

    internal State(string path)
    {
        _directory = new DirectoryInfo(Path.Join(path, ".comfyg"));
    }

    public async Task StoreEncryptedAsync<T>(string scope, string key, T data,
        CancellationToken cancellationToken = default)
    {
        var serializedData = await SerializeAsync(data, cancellationToken).ConfigureAwait(false);
        //TODO encrypt
        await StoreAsync(scope, key, serializedData, cancellationToken).ConfigureAwait(false);
    }

    public async Task StoreAsync<T>(string scope, string key, T data, CancellationToken cancellationToken = default)
    {
        var serializedData = await SerializeAsync(data, cancellationToken).ConfigureAwait(false);
        await StoreAsync(scope, key, serializedData, cancellationToken).ConfigureAwait(false);
    }

    private async Task StoreAsync(string scope, string key, byte[] data, CancellationToken cancellationToken)
    {
        if (!_directory.Exists) _directory.Create();

        var content = new Dictionary<string, string>();
        var dataValue = Convert.ToBase64String(data);

        var file = new FileInfo(Path.Join(_directory.FullName, scope));
        if (!file.Exists)
        {
            content.Add(key, dataValue);
        }
        else
        {
            var result = await ReadAsync(file, cancellationToken).ConfigureAwait(false);
            if (result != null)
            {
                content = result;
                if (content.ContainsKey(key))
                {
                    content[key] = dataValue;
                }
                else
                {
                    content.Add(key, dataValue);
                }
            }
            else
            {
                content.Add(key, dataValue);
            }
        }

        var serializedContent = await SerializeAsync(content, cancellationToken).ConfigureAwait(false);

        await using var writer = file.OpenWrite();
        await writer.WriteAsync(serializedContent, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<Dictionary<string, string>?> ReadAsync(FileInfo file,
        CancellationToken cancellationToken)
    {
        await using var stream = file.OpenRead();
        return await JsonSerializer
            .DeserializeAsync<Dictionary<string, string>>(stream, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    private static async Task<byte[]> SerializeAsync<T>(T data, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, data, cancellationToken: cancellationToken).ConfigureAwait(false);
        stream.Position = 0;
        return stream.ToArray();
    }
}