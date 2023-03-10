namespace Comfyg.Authentication.Abstractions;

public interface IClient
{
    string ClientId { get; set; }
    
    string ClientSecret { get; set; }
    
    string FriendlyName { get; set; }
}