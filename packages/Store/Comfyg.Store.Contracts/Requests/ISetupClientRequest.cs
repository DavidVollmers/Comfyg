using Microsoft.AspNetCore.Http;

namespace Comfyg.Store.Contracts.Requests;

/// <summary>
/// Request object used to setup a new Comfyg client.
/// </summary>
public interface ISetupClientRequest : IClient
{
    public IFormFile? ClientSecretPublicKey { get; }
    
    // Must be public to use with [FromForm]
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class Form : ISetupClientRequest
    {
        public string ClientId { get; init; } = null!;

        public string ClientSecret => null!;

        public string FriendlyName { get; init; } = null!;

        public bool IsAsymmetric => ClientSecretPublicKey != null;

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public IFormFile? ClientSecretPublicKey { get; init; } 
    }
}
