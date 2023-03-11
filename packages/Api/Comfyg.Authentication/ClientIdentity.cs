using System.Security.Claims;
using System.Security.Principal;
using Comfyg.Authentication.Abstractions;
using Comfyg.Contracts.Authentication;

namespace Comfyg.Authentication;

internal class ClientIdentity : ClaimsIdentity, IClientIdentity
{
    public IClient Client { get; }

    public ClientIdentity(IClient client, IIdentity identity) : base(identity)
    {
        Client = client;
    }
}