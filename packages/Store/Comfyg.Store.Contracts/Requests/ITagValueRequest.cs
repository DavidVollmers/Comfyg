using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Comfyg.Store.Contracts.Requests;

/// <summary>
/// Request object used to tag a Comfyg value.
/// </summary>
[JsonConverter(typeof(ContractConverter<ITagValueRequest, Implementation>))]
public interface ITagValueRequest
{
    /// <summary>
    /// The key of the Comfyg value to tag.
    /// </summary>
    [Required]
    [MaxLength(1024)]
    [TechnicalIdentifier]
    string Key { get; }

    /// <summary>
    /// The version of the Comfyg value to tag.
    /// </summary>
    string Version { get; }

    /// <summary>
    /// The tag of the Comfyg value.
    /// </summary>
    [Required]
    [MaxLength(256)]
    [TechnicalIdentifier]
    string Tag { get; }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class Implementation : ITagValueRequest
    {
        public string Key { get; init; } = null!;
        
        public string Version { get; init; } = null!;
        
        public string Tag { get; init; } = null!;
    }
}
