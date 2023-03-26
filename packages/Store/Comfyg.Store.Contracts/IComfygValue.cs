using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Comfyg.Store.Contracts;

public interface IComfygValue
{
    [Required]
    [MaxLength(256)]
    [TechnicalIdentifier]
    string Key { get; }

    [Required][MaxLength(1024)] string Value { get; }

    [ValidateNever] string Version { get; }

    [ValidateNever] DateTimeOffset CreatedAt { get; }

    [JsonIgnore][ValidateNever] string Hash { get; }
}
