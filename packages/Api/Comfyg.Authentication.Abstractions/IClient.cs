namespace Comfyg.Authentication.Abstractions;

public interface IClient
{
    string ClientId { get; }
    
    string ClientSecret { get; }
    
    string FriendlyName { get; }
}