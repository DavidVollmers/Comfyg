namespace Comfyg.Contracts.Authentication;

public sealed class Client : IClient
{
    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string FriendlyName { get; set; }
}