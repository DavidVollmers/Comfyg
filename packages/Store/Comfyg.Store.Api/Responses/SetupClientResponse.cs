using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Responses;

namespace Comfyg.Store.Api.Responses;

internal class SetupClientResponse : ISetupClientResponse
{
    public IClient Client { get; }

    public string? ClientSecret { get; }

    public SetupClientResponse(IClient client, string? clientSecret = null)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
        ClientSecret = clientSecret;
    }
}
