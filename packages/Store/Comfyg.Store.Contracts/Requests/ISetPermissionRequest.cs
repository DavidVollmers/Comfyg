using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Comfyg.Store.Contracts.Requests;

/// <summary>
/// Request object to set permissions on a Comfyg value for a specific client.
/// </summary>
[JsonConverter(typeof(ContractConverter<ISetPermissionRequest, Implementation>))]
public interface ISetPermissionRequest
{
    /// <summary>
    /// The ID of the client to set the permission for.
    /// </summary>
    [Required]
    [MaxLength(64)]
    [TechnicalIdentifier]
    string ClientId { get; }
    
    /// <summary>
    /// The key of the Comfyg value to set the permission for.
    /// </summary>
    [Required]
    [MaxLength(1024)]
    [TechnicalIdentifier]
    string Key { get; }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class Implementation : ISetPermissionRequest
    {
        public string ClientId { get; init; } = null!;

        public string Key { get; init; } = null!;
    }
}
