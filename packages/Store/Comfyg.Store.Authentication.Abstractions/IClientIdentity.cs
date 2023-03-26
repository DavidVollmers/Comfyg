using Comfyg.Store.Contracts.Authentication;

namespace Comfyg.Store.Authentication.Abstractions;

public interface IClientIdentity
{
    IClient Client { get; }
}
