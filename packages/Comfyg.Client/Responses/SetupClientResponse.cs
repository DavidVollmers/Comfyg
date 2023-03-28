using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Responses;

namespace Comfyg.Client.Responses;

internal class SetupClientResponse : ISetupClientResponse
{
    public IClient Client { get; init; } = null!;

    public string ClientSecret { get; init; } = null!;
}
