using Comfyg.Store.Contracts.Requests;

namespace Comfyg.Client.Requests;

internal class TagValueRequest : ITagValueRequest
{
    public string Key { get; }
    
    public string Version { get; }
    
    public string Tag { get; }

    public TagValueRequest(string key, string tag, string version)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Tag = tag ?? throw new ArgumentNullException(nameof(tag));
        Version = version ?? throw new ArgumentNullException(nameof(version));
    }
}
