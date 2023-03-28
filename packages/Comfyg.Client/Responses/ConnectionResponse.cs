using Comfyg.Store.Contracts.Authentication;
using Comfyg.Store.Contracts.Responses;

namespace Comfyg.Client.Responses;

internal class ConnectionResponse : IConnectionResponse
{
    public IClient Client { get; init; } = null!;
}
