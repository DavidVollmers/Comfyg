using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Comfyg.Store.Contracts.Authentication;

/// <summary>
/// A Comfyg client used for authentication and authorization.
/// </summary>
public interface IClient
{
    /// <summary>
    /// The client ID.
    /// </summary>
    [Required]
    [MaxLength(64)]
    [TechnicalIdentifier]
    string ClientId { get; }

    /// <summary>
    /// The client secret.
    /// </summary>
    [JsonIgnore]
    [ValidateNever]
    [IgnoreDataMember]
    string ClientSecret { get; }

    /// <summary>
    /// The user friendly display name of the client.
    /// </summary>
    [Required]
    [MaxLength(256)]
    string FriendlyName { get; }
}
