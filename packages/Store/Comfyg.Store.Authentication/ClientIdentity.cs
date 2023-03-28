using System.Security.Claims;
using System.Security.Principal;
using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Contracts;

namespace Comfyg.Store.Authentication;

internal class ClientIdentity : ClaimsIdentity, IClientIdentity
{
    public IClient Client { get; }

    public ClientIdentity(IClient client, IIdentity identity) : base(identity)
    {
        if (identity == null) throw new ArgumentNullException(nameof(identity));

        Client = client ?? throw new ArgumentNullException(nameof(client));
    }
}
