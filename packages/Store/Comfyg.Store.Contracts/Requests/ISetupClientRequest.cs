using Microsoft.AspNetCore.Http;

namespace Comfyg.Store.Contracts.Requests;

/// <summary>
/// Request object used to setup a new Comfyg client.
/// </summary>
public interface ISetupClientRequest : IClient
{
    /// <summary>
    /// Optional RSA public key to create an asymmetric client instead of a symmetric one.
    /// </summary>
    public IFormFile? ClientSecretPublicKey { get; }

    /// <summary>
    /// Optional encryption key which will be used to de- and encrypt Comfyg values with the created client.
    /// </summary>
    public IFormFile? EncryptionKey { get; }
    
    // Must be public to use with [FromForm]
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class Form : ISetupClientRequest
    {
        public string ClientId { get; init; } = null!;

        public string ClientSecret => null!;

        public string FriendlyName { get; init; } = null!;

        public bool IsAsymmetric => ClientSecretPublicKey != null;

        public IFormFile? ClientSecretPublicKey { get; init; } = null;

        public IFormFile? EncryptionKey { get; init; } = null;
    }
}
