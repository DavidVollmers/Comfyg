using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
    [Required]
    string Value { get; }

    /// <summary>
    /// The version of the Comfyg value.
    /// </summary>
    string Version { get; }

    /// <summary>
    /// The time when the Comfyg value was created.
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// A hash value to identity the Comfyg value.
    /// </summary>
    [JsonIgnore]
    string Hash { get; }

    /// <summary>
    /// The tag of the Comfyg value.
    /// </summary>
    [JsonIgnore]
    string? Tag { get; }
}
