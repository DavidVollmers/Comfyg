using Comfyg.Contracts.Authentication;

namespace Comfyg.Authentication.Abstractions;

public interface IClientIdentity
{
    IClient Client { get; }
}