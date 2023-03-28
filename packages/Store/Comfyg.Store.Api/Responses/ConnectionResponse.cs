using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Responses;

namespace Comfyg.Store.Api.Responses;

internal class ConnectionResponse : IConnectionResponse
{
    public IClient Client { get; }

    public ConnectionResponse(IClient client)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
    }
}
