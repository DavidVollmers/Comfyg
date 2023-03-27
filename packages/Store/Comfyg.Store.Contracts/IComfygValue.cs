using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Comfyg.Store.Contracts;

/// <summary>
/// A Comfyg value.
/// </summary>
public interface IComfygValue
{
    /// <summary>
    /// The key of the Comfyg value.
    /// </summary>
    [Required]
    [MaxLength(1024)]
    [TechnicalIdentifier]
    string Key { get; }
    
    /// <summary>
    /// The Comfyg value.
    /// </summary>
    //TODO define proper max length
    [Required] string Value { get; }
    
    /// <summary>
    /// The version of the Comfyg value.
    /// </summary>
    [ValidateNever] string Version { get; }
    
    /// <summary>
    /// The time when the Comfyg value was created.
    /// </summary>
    [ValidateNever] DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// A hash value to identity the Comfyg value.
    /// </summary>
    [JsonIgnore][ValidateNever] string Hash { get; }
}
