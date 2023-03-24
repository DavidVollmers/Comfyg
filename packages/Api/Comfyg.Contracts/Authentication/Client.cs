namespace Comfyg.Contracts.Authentication;

internal class Client : IClient
{
    public string ClientId { get; init; } = null!;

    public string ClientSecret { get; init; } = null!;

    public string FriendlyName { get; init; } = null!;
}
