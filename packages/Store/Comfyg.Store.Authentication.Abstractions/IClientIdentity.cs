using Comfyg.Store.Contracts;

namespace Comfyg.Store.Authentication.Abstractions;

public interface IClientIdentity
{
    IClient Client { get; }
    
    bool IsSystemClient { get; }
}
