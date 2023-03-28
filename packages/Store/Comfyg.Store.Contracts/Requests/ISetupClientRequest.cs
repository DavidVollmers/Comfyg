using System.ComponentModel.DataAnnotations;

namespace Comfyg.Store.Contracts.Requests;

/// <summary>
/// Request object used to setup a new Comfyg client.
/// </summary>
public interface ISetupClientRequest
{
    /// <summary>
    /// The Comfyg client to register. 
    /// </summary>
    [Required]
    IClient Client { get; }
}
