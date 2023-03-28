using Comfyg.Store.Contracts.Authentication;
using Comfyg.Store.Contracts.Requests;

namespace Comfyg.Client.Requests;

internal class SetupClientRequest : ISetupClientRequest
{
    public IClient Client { get; }

    public SetupClientRequest(IClient client)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
    }
}
