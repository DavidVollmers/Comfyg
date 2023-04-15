using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Comfyg.Store.Contracts.Requests;

/// <summary>
/// Request object to set permissions for a specific client.
/// </summary>
[JsonConverter(typeof(ContractConverter<ISetPermissionsRequest, Implementation>))]
public interface ISetPermissionsRequest
{
    /// <summary>
    /// The ID of the client to set the permissions for.
    /// </summary>
    [Required]
    [MaxLength(64)]
    [TechnicalIdentifier]
    string ClientId { get; }

    /// <summary>
    /// The kind of permissions to set for the client.
    /// </summary>
    [Required] Permissions Permissions { get; }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class Implementation : ISetPermissionsRequest
    {
        public string ClientId { get; init; } = null!;

        public Permissions Permissions { get; } = Permissions.Read;
    }
}
