using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Comfyg.Store.Contracts.Requests;

/// <summary>
/// Request object used to setup a new Comfyg client.
/// </summary>
public interface ISetupClientRequest : IClient
{
    // Must be public to use with [FromForm]
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class Form : ISetupClientRequest
    {
        public string ClientId { get; init; } = null!;

        public string ClientSecret => null!;

        public string FriendlyName { get; init; } = null!;
    }
}
